using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.WallpaperJet
{
    public class JetControl : PaperJetControlAbstract
    {


        new Rigidbody2D rigidbody;
        public Animator animator;
        float prevSign = 1;
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }
        // Start is called before the first frame update
        void Start()
        {

        }
        private void Update()
        {
            float y=rigidbody.velocity.y;
            if (inHeat)
            {
                y = Mathf.Clamp(y+ 20*Time.deltaTime, -4,20);
            }
            else
            {
                y = y = Mathf.Clamp(y-4* Time.deltaTime, -10, 20);
            }
            float v = Input.GetAxis("Horizontal");
            rigidbody.velocity = v * Vector2.right * 20+y*Vector2.up;
            float sign = Mathf.Sign(v);
            if (sign != prevSign && v != 0 && prevSign != 0)
            {
                animator.SetTrigger("turn");
            }
            if (v != 0)
            {

                prevSign = Mathf.Sign(v);
            }
            print(sign);
        }
        // Update is called once per frame
        void FixedUpdate()
        {

        }
    }
}