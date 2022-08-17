using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UKAUserList : MonoBehaviour
{

    (string username, string password)[] users = new (string username, string password)[]
    {
        ("uka01","s2t7xn;"),
        ("uka02","5829yn"),
        ("uka03","n8chne"),
        ("uka04","xs68ap"),
        ("uka05","2mgzkg"),
        ("uka06","rxks6u"),
        ("uka07","zj3kss"),
        ("uka08","smx8ts"),
        ("uka09","8twaax"),
        ("uka10","3g8t9c"),
        ("uka11","s6wbqk"),
        ("uka12","j8v22k"),
        ("uka13","y3yz5z"),
        ("uka14","p3d3gx"),
        ("uka15","cbje3f"),
        ("uka16","ye3a3w"),
        ("uka17","9zgdaw"),
        ("uka18","v5zds5"),
        ("uka19","9bs6fv"),
        ("uka20","p63yau"),
        ("uka21","6sm92x"),
        ("uka22","sw7ame"),
        ("uka23","mw6y73"),
        ("uka24","u5xqwv"),
        ("uka25","h6a22x"),
        ("uka26","b548du"),
        ("uka27","yjfd6j"),
        ("uka28","rh42qp"),
        ("uka29","6cf3ks"),
        ("uka30","ek2f2w"),
        ("uka31","48g7ga"),
        ("uka32","dp3pze"),
        ("uka33","rk78xd"),
        ("uka34","sng7vk"),
        ("uka35","8jz84d"),
        ("uka36","usm2c2"),
        ("uka37","b44s6a"),
        ("uka38","fae63v"),
        ("uka39","77gveq"),
        ("uka40","65entd"),
    };

    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in users)
        {
            //LocalDatabaseManager.instance.InsertUser(item.username, item.password);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
