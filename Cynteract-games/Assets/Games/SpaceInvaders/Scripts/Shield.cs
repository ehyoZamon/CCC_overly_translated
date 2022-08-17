using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.SpaceInvaders
{
    public class Shield : MonoBehaviour
    {
        public CircleCollider2D circleCollider;
        public int maxHP;
        [ShowInInspector]
        [ReadOnly]
        private int hp;
        public SpriteRenderer cracks;
        public GameObject destructionEffect;
        private void DestroyDangers(Collider2D collider)
        {
            Enemy enemy = collider.gameObject.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.Destruction();
                hp -= 5;
            }
            Brick brick = collider.gameObject.GetComponent<Brick>();
            if (brick)
            {
                brick.Destruction();
                hp-=5;

            }
            Projectile projectile = collider.gameObject.GetComponent<Projectile>();
            if (projectile)
            {
                projectile.Destruction();
                hp -= 10;

            }
            Obstacle obstacle = collider.gameObject.GetComponent<Obstacle>();
            if (obstacle)
            {
                obstacle.Destruction();
                hp-=30;

            }
        }

        public void Activate()
        {
            hp = maxHP;
            gameObject.SetActive(true);
            UpdateHp();
            LevelController.instance.DestroyEnemiesInCircle(transform.position, 10);

        }
        public void Deactivate()
        {
            hp = 0;
            gameObject.SetActive(false);
            LevelController.instance.DestroyEnemiesInCircle(transform.position, 10);
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }
        private void Start()
        {
            Activate();
        }
        private void Update()
        {
            var colliders=Physics2D.OverlapCircleAll(transform.position, circleCollider.radius*transform.lossyScale.x);
            foreach (var item in colliders)
            {
                DestroyDangers(item);
            }
            UpdateHp();
        }

        private void UpdateHp()
        {
            if (hp<=0)
            {
                Deactivate();
            }
            float amount = 1 - ((float)hp / maxHP);
            Color color = cracks.color;
            color.a = amount;
            cracks.color = color;
        }


    }
}