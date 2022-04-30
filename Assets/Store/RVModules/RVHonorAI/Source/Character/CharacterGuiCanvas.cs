// Created by Ronis Vision. All rights reserved
// 02.04.2021.

using RVModules.RVLoadBalancer;
using UnityEngine;
using UnityEngine.UI;

namespace RVHonorAI
{
    public class CharacterGuiCanvas : MonoBehaviour
    {
        #region Fields

        private ICharacter character;

        [SerializeField]
        private Image healthImage;

        [SerializeField]
        private Text infoText;

        [SerializeField]
        private Text healthNumberText;

        private Transform camTransform;
        private new Transform transform;

        private bool visible = true;

        [SerializeField]
        private float visibilityDistance = 8;

        #endregion

        #region Not public methods

        private void Awake()
        {
            transform = base.transform;
            character = transform.parent.GetComponent<ICharacter>();
            infoText.text = character.Transform.name;
            camTransform = Camera.main.transform;
            character.OnKilled.AddListener(() => Destroy(gameObject));

            LoadBalancerSingleton.Instance.Register(this, CheckDistance, 5);
            LoadBalancerSingleton.Instance.Register(this, Tick, 20);
            Hide();
        }

        private void Tick(float _dt)
        {
            if (!visible) return;
            var camPos = camTransform.position;
            var position = transform.position;
            transform.LookAt(position + (position - camPos));
            healthImage.fillAmount = character.HitPoints / character.MaxHitPoints;
            healthNumberText.text = $"{(int) character.HitPoints}/{(int) character.MaxHitPoints}HP";

            //transform.localScale = Vector3.one * (.008f + Vector3.Distance(position, camPos) * 0.001f);
        }

        private void OnDestroy() => LoadBalancerSingleton.Instance?.Unregister(this);

        private void CheckDistance(float _dt)
        {
            if (!visible)
            {
                if (Vector3.Distance(transform.position, camTransform.position) < visibilityDistance) Show();
            }
            else
            {
                if (Vector3.Distance(transform.position, camTransform.position) > visibilityDistance) Hide();
            }
        }

        private void Hide()
        {
            visible = false;
            gameObject.SetActive(false);
        }

        private void Show()
        {
            visible = true;
            Tick(0);
            gameObject.SetActive(true);
        }

        #endregion
    }
}