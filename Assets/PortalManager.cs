using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{

    public static PortalManager instance;

    public GameObject player;
    public GenerateMap mapGenerator;
    public List<GameObject> portals;

    private PortalManager() {
        portals = new List<GameObject>();
        instance = this;
    }

    public void Start() {

    }

    public void removePortals() {
        for (int i = 0; i < portals.Count; i++) {
            Destroy(portals[i]);
        }

        portals.Clear();
    }

    public void addPortal(int x, int y) {
        Object portalPrefab = Resources.Load("Portal");
        GameObject portal = Instantiate(portalPrefab, new Vector3(x + 1, y + 1, 0), Quaternion.identity) as GameObject;
        portal.GetComponent<PortalScript>().player = player;
        portal.GetComponent<PortalScript>().mapGenerator = mapGenerator;
        portal.transform.parent = gameObject.transform;
        portals.Add(portal);
    }

}
