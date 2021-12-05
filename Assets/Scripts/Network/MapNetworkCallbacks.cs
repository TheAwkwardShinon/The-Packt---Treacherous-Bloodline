using System.Collections.Generic;
using UdpKit;
using UnityEngine;
using System;
using Bolt;
using System.Collections;

namespace ThePackt
{
    public class MapNetworkCallbacks : NetworkCallbacks
    {
        public Utils.PrefabAssociation[] enemyPrefabs;
        public Vector2 enemySpawnPos;
        public Utils.VectorAssociation[] playersSpawnPositions;
        [SerializeField] private GameObject _timeManagerPrefab;
        [SerializeField] private Canvas _blackScreenCanvas;
        [SerializeField] private Canvas _fogOfWarCanvas;
        [SerializeField] private float _loadingScreenTime;
        private BoltEntity _timeManager;
        private Player _player;

        #region callbacks

        public override void SceneLoadLocalDone(string scene, IProtocolToken token)
        {
            _fogOfWarCanvas.gameObject.SetActive(_selectedData.GetFogEnabled());

            _player = _selectedData.GetPlayerScript();
            //_player.ActivateFogCircle();

            List<BoltEntity> players = new List<BoltEntity>();
            foreach (BoltEntity ent in BoltNetwork.Entities)
            {
                if(ent.gameObject.GetComponent<Player>() != null)
                {
                    players.Add(ent);

                    if (ent.IsOwner)
                    {
                        /*
                        _player = _selectedData.GetPlayerScript();
                        _player.ActivateFogCircle();
                        */

                        foreach (Utils.VectorAssociation assoc in playersSpawnPositions)
                        {
                            if (assoc.name == _selectedData.GetCharacterSelected())
                            {
                                ent.gameObject.transform.position = assoc.position;
                                Camera.main.GetComponent<CameraFollow>().SetFollowTransform(ent.gameObject.transform);
                            }
                        }
                    }
                }
            }

            if (BoltNetwork.IsServer)
            {
                MainQuest mainQuest = MainQuest.Instance;
                if (mainQuest)
                {
                    mainQuest.SetPlayers(players);
                }

                _timeManager = BoltNetwork.Instantiate(_timeManagerPrefab, Vector3.zero, Quaternion.identity);
                _timeManager.GetComponent<TimerManager>().SetStartTime(BoltNetwork.ServerTime);
            }

            StartCoroutine("LoadingScreen");
        }

        IEnumerator LoadingScreen()
        {
            yield return new WaitForSeconds(_loadingScreenTime);

            _player.SetEnabledInput(true);
            _blackScreenCanvas.gameObject.SetActive(false);
        }

        public override void BoltShutdownBegin(AddCallback registerDoneCallback, UdpConnectionDisconnectReason disconnectReason)
        {
            base.BoltShutdownBegin(registerDoneCallback, disconnectReason);
            _blackScreenCanvas.gameObject.SetActive(true);
        }

        public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
        {
            Debug.Log("[CONNECTIONLOG] connect request");

            BoltNetwork.Refuse(endpoint);
        }

        public override void OnEvent(PlayerAttackHitEvent evnt)
        {
            Debug.LogError("[HEALTH] attack hit with damage: " + evnt.Damage);

            if (BoltNetwork.IsServer)
            {
                Debug.Log("[NETWORKLOG] server received hit event");

                //if received by the server
                //if the server itself was hit apply damage to its player
                Debug.Log("[NETWORKLOG] " + evnt.HitNetworkId + " was hit");
                BoltEntity hitEntity = BoltNetwork.FindEntity(evnt.HitNetworkId);
                if(hitEntity != null)
                {
                    if (hitEntity.IsOwner)
                    {
                        Debug.Log("[NETWORKLOG] server was hit, applying damage");
                        _player.ApplyDamage(evnt.Damage);
                    }
                    else
                    {
                        //otherwise redirect the event to the client that was hit

                        Debug.Log("[NETWORKLOG] server redirect to connection: " + hitEntity.Source.ConnectionId);

                        var newEvnt = PlayerAttackHitEvent.Create(hitEntity.Source);
                        newEvnt.HitNetworkId = evnt.HitNetworkId;
                        newEvnt.Damage = evnt.Damage;
                        newEvnt.Send();
                    }
                }
            }
            else
            {
                //if received by the client, apply damage to the player of which the client is owner
                Debug.Log("[NETWORKLOG] client was hit. my: " + _player.entity.NetworkId + "  other: " + evnt.HitNetworkId);

                if (_player.entity.NetworkId.Equals(evnt.HitNetworkId))
                {
                    Debug.Log("[NETWORKLOG] client was hit, applying damage");
                    _player.ApplyDamage(evnt.Damage);
                }
            }
        }

        public override void OnEvent(EnemyAttackHitEvent evnt)
        {
            Debug.Log("[HEALTH] enemy attack hit with damage: " + evnt.Damage);

            //if received by the server, apply damage to the enemy with the network id stored in the event
            if (BoltNetwork.IsServer)
            {
                Debug.Log("[NETWORKLOG] server received enemy hit event");

                BoltEntity entity = BoltNetwork.FindEntity(evnt.HitNetworkId);
                Enemy enemy = entity.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Debug.Log("[PROTOTYPE] id " + evnt.AttackerNetworkId);

                    BoltEntity attacker = BoltNetwork.FindEntity(evnt.AttackerNetworkId);
                    enemy.ApplyDamage(evnt.Damage, attacker.GetComponent<Player>());
                }
            }
        }

        public override void OnEvent(ObjectiveHitEvent evnt)
        {
            Debug.Log("[OBJECTIVE] objective hit with damage: " + evnt.Damage);

            //if received by the server, apply damage to the objective with the network id stored in the event
            if (BoltNetwork.IsServer)
            {
                Debug.Log("[NETWORKLOG] server received objective hit event");

                BoltEntity entity = BoltNetwork.FindEntity(evnt.HitNetworkId);
                Objective objective = entity.GetComponent<Objective>();
                if (objective != null)
                {
                    objective.ApplyDamage(evnt.Damage);
                }
            }
        }

        public override void OnEvent(ImpostorEvent evnt)
        {
            //Debug.Log("[MAIN] received impostor event: " + _player.entity.NetworkId + "   " + evnt.ImpostorNetworkID);

            _player = _selectedData.GetPlayerScript();
            Debug.Log("[MAIN] received impostor event my: " + _player == null);
            Debug.Log("[MAIN] received impostor event imp: " + evnt.ImpostorNetworkID);

            if (_player.entity.NetworkId.Equals(evnt.ImpostorNetworkID))
            {
                Debug.Log("[MAIN] i'm the fucking impostor");

                _player.SetIsImpostor(true);
                Debug.Log("[MAIN] client imp" + _player.isImpostor());
            }
        }

        public override void OnEvent(ModifyTimerEvent evnt)
        {
            if (BoltNetwork.IsServer)
            {
                if (evnt.MustBeAdded)
                {
                    _timeManager.GetComponent<TimerManager>().addTime(evnt.Amount);
                }
                else
                {
                    _timeManager.GetComponent<TimerManager>().subTime(evnt.Amount);
                }
            }
        }

        public override void OnEvent(StartHealingEvent evnt)
        {
            if (BoltNetwork.IsServer)
            {
                if (_player.entity.NetworkId.Equals(evnt.TargetPlayerNetworkID))
                {
                    _player.SetIsBeingHealed(true);
                }
                else {
                    BoltEntity entity = BoltNetwork.FindEntity(evnt.TargetPlayerNetworkID);
                    var newEvnt = StartHealingEvent.Create(entity.Source);
                    newEvnt.TargetPlayerNetworkID = evnt.TargetPlayerNetworkID;
                    newEvnt.Send();
                }
            }
            else{
                if (_player.entity.NetworkId.Equals(evnt.TargetPlayerNetworkID))
                {
                    _player.SetIsBeingHealed(true);
                }
            }
        }

         public override void OnEvent(HealEvent evnt)
        {
             if (BoltNetwork.IsServer)
            {
                BoltEntity entity = BoltNetwork.FindEntity(evnt.TargetPlayerNetworkID);

                 if(entity != null)
                {
                    if (entity.IsOwner)
                    {
                        Debug.Log("[NETWORKLOG] server was hit, applying damage");
                        _player.Heal();
                    }
                    else
                    {
                        var newEvnt = HealEvent.Create(entity.Source);
                        newEvnt.TargetPlayerNetworkID = evnt.TargetPlayerNetworkID;
                        newEvnt.Send();
                    }
                }
            }else{
                if (_player.entity.NetworkId.Equals(evnt.TargetPlayerNetworkID))
                {
                    _player.Heal();
                     Debug.Log("[INTERACTION] HEAL CALLBACK CORRECTLY CALLED");
                }
            }
        }

        public override void OnEvent(FountainHealEvent evnt)
        {
            if (BoltNetwork.IsClient)
            {
                if (_player.entity.NetworkId.Equals(evnt.TargetPlayerNetworkID))
                {
                    _player.FountainHeal(evnt.Amount);
                }
            }
        }

        public override void OnEvent(ApplicateSlowDebuffEvent evnt)
        {
            if (BoltNetwork.IsServer)
            {
                Debug.LogError("[SLOW BULLET] sono il server");
                if (_player.entity.NetworkId.Equals(evnt.TargetPlayerNetworkID))
                {
                    Debug.LogError("[SLOW BULLET] e sono stato colpito io");
                    _player.ApplicateSlow(evnt.TimeOfSlow);
                    _player.ApplyDamage(evnt.Damage);
                }
                else {
                    Debug.LogError("[SLOW BULLET] il target è un client, ora gli inoltro il colpo");
                    BoltEntity entity = BoltNetwork.FindEntity(evnt.TargetPlayerNetworkID);
                    var newEvnt = ApplicateSlowDebuffEvent.Create(entity.Source);
                    newEvnt.TargetPlayerNetworkID = evnt.TargetPlayerNetworkID;
                    newEvnt.TimeOfSlow = evnt.TimeOfSlow;
                    newEvnt.Send();
                }
            }
            else{
                if (_player.entity.NetworkId.Equals(evnt.TargetPlayerNetworkID))
                {
                    Debug.LogError("[SLOW BULLET] client colpito e affondato");
                    _player.ApplicateSlow(evnt.TimeOfSlow);
                    _player.ApplyDamage(evnt.Damage);
                }
            }
        }

        public override void OnEvent(EnemySlowEvent evnt)
        {
            if (BoltNetwork.IsServer)
            {
                Debug.Log("[HEALTH] enemy attack hit with damage: " + evnt.Damage);

                BoltEntity entity = BoltNetwork.FindEntity(evnt.HitNetworkId);
                Enemy enemy = entity.GetComponent<Enemy>();
                if (enemy != null)
                {
                    BoltEntity attacker = BoltNetwork.FindEntity(evnt.AttackerNetworkId);
                    enemy.ApplyDamage(evnt.Damage, attacker.GetComponent<Player>());
                    enemy.ApplySlow(evnt.SlowTime);
                }
            }
        }

        public override void OnEvent(ApplicateFogOfWarDebuffEvent evnt)
        {
            if (BoltNetwork.IsServer)
            {
                if (_player.entity.NetworkId.Equals(evnt.TargetPlayerNetworkID))
                {
                    _player.ApplicateFogDebuff(evnt.TimeOfDebuff,evnt.CircleSize);
                    _player.ApplyDamage(evnt.Damage);
                }
                else {
                    BoltEntity entity = BoltNetwork.FindEntity(evnt.TargetPlayerNetworkID);
                    var newEvnt = ApplicateFogOfWarDebuffEvent.Create(entity.Source);
                    newEvnt.TargetPlayerNetworkID = evnt.TargetPlayerNetworkID;
                    newEvnt.TimeOfDebuff = evnt.TimeOfDebuff;
                    newEvnt.CircleSize = evnt.CircleSize;
                    newEvnt.Damage = evnt.Damage;
                    newEvnt.Send();
                }
            }
            else{
                if (_player.entity.NetworkId.Equals(evnt.TargetPlayerNetworkID))
                {
                    _player.ApplicateFogDebuff(evnt.TimeOfDebuff,evnt.CircleSize);
                    _player.ApplyDamage(evnt.Damage);
                }
            }
        }

        public override void OnEvent(EnemyStunEvent evnt)
        {
            if (BoltNetwork.IsServer)
            {
                Debug.Log("[HEALTH] enemy attack hit with damage: " + evnt.Damage);

                BoltEntity entity = BoltNetwork.FindEntity(evnt.HitNetworkId);
                Enemy enemy = entity.GetComponent<Enemy>();
                if (enemy != null)
                {
                    BoltEntity attacker = BoltNetwork.FindEntity(evnt.AttackerNetworkId);
                    enemy.ApplyDamage(evnt.Damage, attacker.GetComponent<Player>());
                    enemy.Stun(evnt.StunTime);
                }
            }
        }

        public override void OnEvent(ApplicateDamageReductionEvent evnt)
        {
            if (BoltNetwork.IsServer)
            {
                if (_player.entity.NetworkId.Equals(evnt.TargetPlayerNetworkID))
                {
                    _player.ApplicateDamageReductionDebuff(evnt.TimeOfDebuff,evnt.DamageReduction);
                    _player.ApplyDamage(evnt.Damage);
                }
                else {
                    BoltEntity entity = BoltNetwork.FindEntity(evnt.TargetPlayerNetworkID);
                    var newEvnt = ApplicateDamageReductionEvent.Create(entity.Source);
                    newEvnt.TargetPlayerNetworkID = evnt.TargetPlayerNetworkID;
                    newEvnt.TimeOfDebuff = evnt.TimeOfDebuff;
                    newEvnt.DamageReduction = evnt.DamageReduction;
                    newEvnt.Damage = evnt.Damage;
                    newEvnt.Send();
                }
            }
            else{
                if (_player.entity.NetworkId.Equals(evnt.TargetPlayerNetworkID))
                {
                     _player.ApplicateDamageReductionDebuff(evnt.TimeOfDebuff,evnt.DamageReduction);
                    _player.ApplyDamage(evnt.Damage);
                }
            }
        }

        public override void OnEvent(EnemyDamageReductionEvent evnt)
        {
            if (BoltNetwork.IsServer)
            {
                Debug.Log("[HEALTH] enemy attack hit with damage: " + evnt.Damage);

                BoltEntity entity = BoltNetwork.FindEntity(evnt.HitNetworkId);
                Enemy enemy = entity.GetComponent<Enemy>();
                if (enemy != null)
                {
                    BoltEntity attacker = BoltNetwork.FindEntity(evnt.AttackerNetworkId);
                    enemy.ApplyDamage(evnt.Damage, attacker.GetComponent<Player>());
                    enemy.ApplyDamageReduction(evnt.DamageTime, evnt.DamageReduction);
                }
            }
        }

        public override void OnEvent(ApplicateforceInDirection evnt)
        {
            if (BoltNetwork.IsServer)
            {
                if (_player.entity.NetworkId.Equals(evnt.TargetPlayerNetworkID))
                {
                    _player.ApplicateForce(evnt.Direction, evnt.Power);
                    _player.ApplyDamage(evnt.Damage);
                }
                else {
                    BoltEntity entity = BoltNetwork.FindEntity(evnt.TargetPlayerNetworkID);
                    var newEvnt = ApplicateforceInDirection.Create(entity.Source);
                    newEvnt.TargetPlayerNetworkID = evnt.TargetPlayerNetworkID;
                    newEvnt.Direction = evnt.Direction;
                    newEvnt.Damage = evnt.Damage;
                    newEvnt.Send();
                }
            }
            else{
                if (_player.entity.NetworkId.Equals(evnt.TargetPlayerNetworkID))
                {
                    _player.ApplicateForce(evnt.Direction, evnt.Power);
                    _player.ApplyDamage(evnt.Damage);
                }
            }
        }

        public override void OnEvent(EnemyKnockbackEvent evnt)
        {
            if (BoltNetwork.IsServer)
            {
                Debug.Log("[HEALTH] enemy attack hit with damage: " + evnt.Damage);

                BoltEntity entity = BoltNetwork.FindEntity(evnt.HitNetworkId);
                Enemy enemy = entity.GetComponent<Enemy>();
                if (enemy != null)
                {
                    BoltEntity attacker = BoltNetwork.FindEntity(evnt.AttackerNetworkId);
                    enemy.ApplyDamage(evnt.Damage, attacker.GetComponent<Player>());
                    enemy.ApplyKnockback(evnt.Direction, evnt.KnockbackPower);
                }
            }
        }

        public override void OnEvent(TransformationEvent evnt)
        {
             BoltEntity entity = BoltNetwork.FindEntity(evnt.TargetPlayerNetworkID);
            Player player =entity.GetComponent<Player>();
            player.GetHumanObject().SetActive(false);
            player.GetWolfObject().SetActive(true);
            player.SetTransformationVFX(false);
        }

        public override void OnEvent(DetransformationEvent evnt)
        {
            BoltEntity entity = BoltNetwork.FindEntity(evnt.TargetPlayerNetworkID);
            
            Player player =entity.GetComponent<Player>();
            player.GetWolfObject().SetActive(false);
            player.GetHumanObject().SetActive(true);
            player.SetTransformationVFX(false);
        }

        public override void OnEvent(ActivateVFXEvent evnt)
        {
            BoltEntity entity = BoltNetwork.FindEntity(evnt.TargetPlayerNetworkID);
            Player player =entity.GetComponent<Player>();
            switch(evnt.VFXName){
                case "transformation": player.SetTransformationVFX(evnt.Active);
                                        break;
                case "hit": player.SetHitVFX(evnt.Active);
                            break;
                case "special": player.SetSpecialVFX(evnt.Active);
                            break;
            }
        }

        #endregion

        #region methods

        #endregion

        #region getters
        public Utils.VectorAssociation[] GetPlayerSpawnPositions()
        {
            return playersSpawnPositions;
        }
        #endregion
    }
}
