using StolenLands.Player;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.AI;
using StolenLands.Projectiles;
using UnityEngine.Serialization;

namespace StolenLands.Enemy
{
    public class EnemyAIntelligence : MonoBehaviour
    {
        #region vars

        [Header("Enemy Stats")] 
        [Range(1, 20)] public float roamSpeed;

        [Range(1, 20)] public float chaseSpeed;
        [SerializeField] float awarenessRange;

        [FormerlySerializedAs("chaseRange")] 
        [SerializeField]
        float currentChaseRange = 5f;

        [SerializeField] float defaultChaseRange;
        [SerializeField] float maxChaseRange;
        [SerializeField] float suspicionTime = 2.5f;
        [SerializeField] float timeToWaitInWaypoint = 2f;
        [SerializeField] float rotationSpeed = 190f;

        public bool canSeePlayer = false;
        public bool isAggro = false;
        [Header("Enemy Combat")] float attackRange;
        [SerializeField] float attackRate = 2f;
        public float baseDamage = 8f;

        [Tooltip("Pink gizmo")] [SerializeField]
        private float warningRadius;

        public EnemyWeapon defaultWeapon = null;
        public bool hasHitPlayer = false;
        public bool isAttacking = false;
        public bool beginAttack = false;

        [Header("Enemy Movement")] List<Transform> points = new List<Transform>();
        private int destPoint = 0;
        public WaypointSystem path;

        [SerializeField] bool followsPath = false;
        //public float remainingDistance = 0.3f;

        // CLASS VARIABLES
        float timeSinceBecameSuspicious = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceLastAttack = Mathf.Infinity;
        float defaultAwarenessRange;
        public bool isDoctor;
        public bool isArcher;
        
        private AudioClip attackSound;

        private Vector3 spawnPos;
        private Quaternion spawnRotation;

        private bool isChangingChaseRange = false;

        // CACHED REFERENCES
        public GameObject targetPlayer;
        PlayerHealthController playerHealthController;
        PlayerCombatController pCombat;
        EnemyController enemyController;
        NavMeshAgent enemyNavAgent;
        Animator enemyAnimator;
        EnemyArcher archer;
        EnemyDoctor doctor;
        AudioSource enemyAudioSource;
        FieldOfView foV;
        PlayerControllerV2 playerController;
        #endregion

        void Start()
        {
                
            defaultAwarenessRange = awarenessRange;
            
            spawnPos = transform.position;
            spawnRotation = transform.rotation;

            CacheReferences();

            attackSound = enemyController.attackSound;

            GetEnemyType();

            if (isArcher || isDoctor) attackRange = defaultWeapon.weaponRange;

            if (path != null) points = path.waypoints;

            enemyNavAgent.autoBraking = false;

            GotoNextPoint();
        }

        private void CacheReferences()
        {
            targetPlayer = GameObject.FindGameObjectWithTag("Player");
            playerHealthController = targetPlayer.GetComponent<PlayerHealthController>();
            pCombat = targetPlayer.GetComponent<PlayerCombatController>();
            enemyController = GetComponent<EnemyController>();
            enemyNavAgent = GetComponent<NavMeshAgent>();
            enemyAudioSource = GetComponent<AudioSource>();
            enemyAnimator = GetComponent<Animator>();
            foV = GetComponent<FieldOfView>();
            playerController = targetPlayer.GetComponent<PlayerControllerV2>();
        }

        private void GetEnemyType()
        {
            if (GetComponent<EnemyArcher>())
            {
                archer = GetComponent<EnemyArcher>();
                isArcher = true;
            }

            if (GetComponent<EnemyDoctor>())
            {
                doctor = GetComponent<EnemyDoctor>();
                isDoctor = true;
            }
        }

        void Update()
        {
            if (playerController.playerInvisible || enemyController.IsDead())
                return; // if cheat on then AI loop wont happen

            if (!enemyController.isSlowed && !enemyController.isStunned && targetPlayer == null)
            {
                enemyNavAgent.speed = roamSpeed;
                return;
            }

            if (PlayerMover.instance.IsPlayerCrouching())
            {
                currentChaseRange = 0.5f;
                awarenessRange = 0.5f;
            }
            else
            {
                if (!isChangingChaseRange)
                {
                    StartCoroutine(ChangeChaseRangeRoutine());
                }
            }

            if (enemyController.wasHit || foV.canSeePlayer || GetDistanceToPlayer() < awarenessRange)
            {
                timeSinceLastSawPlayer = 0f;
                isAggro = true;
                FacePlayer();
                WarnFriendsNearby();
                if (GetIfRangeToAttack())
                {
                    enemyNavAgent.isStopped = true;
                    AttackBehaviour();
                }
                else
                {
                    enemyController.EnemyMoveTo(targetPlayer.transform.position);
                }
            }

            if (GetDistanceToPlayer() > maxChaseRange && !enemyController.wasHit)
            {
                if (Vector3.Distance(transform.position, spawnPos) < 1f)
                {
                    return;
                }
                if (!enemyController.wasHit || !foV.canSeePlayer)
                {
                    SuspicionState();
                    
                }
                if (Vector3.Distance(transform.position, spawnPos) < 1.5f)
                {
                    RotateToInitialRotation();
                }
            }

            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceLastAttack += Time.deltaTime;
            timeSinceBecameSuspicious += Time.deltaTime;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, currentChaseRange);
            Gizmos.color = Color.red;
            if (defaultWeapon != null) Gizmos.DrawWireSphere(transform.position, defaultWeapon.weaponRange);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, warningRadius);
        }

        public void FacePlayer()
        {
            var targetRotation = Quaternion.LookRotation(targetPlayer.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        private Transform GotoNextPoint()
        {
            if (points.Count == 0)
                return null;
            

            enemyNavAgent.destination = points[destPoint].position;

            destPoint = (destPoint + 1) % points.Count;
            timeSinceArrivedAtWaypoint = 0;
            
            return points[destPoint].transform;
        }

        private void SuspicionState()
        {
            timeSinceBecameSuspicious = 0;
            StartCoroutine(SuspicionRoutine());
        }

        private void RotateToInitialRotation()
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, spawnRotation, 0.2f);
        }

        IEnumerator SuspicionRoutine()
        {
            isAggro = false;
            foV.canSeePlayer = false;
            enemyNavAgent.isStopped = true;
            enemyAnimator.SetBool("isAttacking", false);
            
            yield return new WaitForSeconds(suspicionTime);
            
            if (!followsPath)
            {
                enemyController.EnemyMoveTo(spawnPos);
            }

            if (followsPath)
            {
                PatrollingState();
            }
            
        }

        private void PatrollingState()
        {
            enemyNavAgent.speed = roamSpeed;
            enemyNavAgent.isStopped = false;
            if (timeSinceArrivedAtWaypoint > timeToWaitInWaypoint)
            {
                if (!enemyNavAgent.pathPending && enemyNavAgent.remainingDistance < 0.5f)
                    GotoNextPoint();
            }
        }

        private void AttackBehaviour()
        {
            if (targetPlayer == null) return;

            if (timeSinceLastAttack > attackRate)
            {
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        private void WarnFriendsNearby()
        {
            Collider[] friends = Physics.OverlapSphere(transform.position, warningRadius);
            for (int i = 0; i < friends.Length; i++)
            {
                if (friends[i].GetComponent<EnemyAIntelligence>() && friends[i].gameObject != this.gameObject)
                {
                    if (targetPlayer != null)
                    {
                        friends[i].GetComponent<EnemyAIntelligence>().beginAttack = true;
                    }
                }
            }
        }

        public void TriggerAttack()
        {
            enemyAnimator.ResetTrigger("stopAttack");
            enemyAnimator.SetBool("isAttacking", true);
            enemyAnimator.SetTrigger("attack");
        }

        public void ProcessHit()
        {
            if (targetPlayer == null) return;

            float distance = Vector3.Distance(transform.position, targetPlayer.transform.position);

            if (distance <= 3f && !isArcher) //makes melee slashes only hit player when less than 3f from enemy
            {
                targetPlayer.GetComponent<PlayerHealthController>().TakeDamage(defaultWeapon.weaponDamage, baseDamage);
            }

            if (isArcher)
            {
                targetPlayer.GetComponent<PlayerHealthController>().TakeDamage(defaultWeapon.weaponDamage, baseDamage);
            }
        }

        public float GetDistanceToPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            return Vector3.Distance(transform.position, player.transform.position);
        }

        public Transform GetNearestFriendTransform(float spellRange)
        {
            Collider[] friends = Physics.OverlapSphere(transform.position, spellRange);
            for (int i = 0; i < friends.Length; i++)
            {
                if (friends[i].gameObject != this.gameObject)
                {
                    return friends[i].transform;
                }
            }
            return null;
        }

        private bool GetIfRangeToAttack()
        {
            return GetDistanceToPlayer() < attackRange;
        }

        public void PlayAttackSound()
        {
            if (isArcher)
            {
                return;
            }
            else
            {
                enemyAudioSource.PlayOneShot(attackSound, enemyController.soundFXVolume);
            }
        }

        public void CancelAttack()
        {
            enemyAnimator.SetBool("isAttacking", false);
            enemyAnimator.SetTrigger("stopAttack");
            targetPlayer = null;
        }

        IEnumerator ChangeChaseRangeRoutine()
        {
            isChangingChaseRange = true;
            yield return new WaitForSeconds(2f);
            currentChaseRange = defaultChaseRange;
            awarenessRange = defaultAwarenessRange;
            isChangingChaseRange = false;
        }
    }
}