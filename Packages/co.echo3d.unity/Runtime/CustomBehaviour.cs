/**************************************************************************
* Copyright (C) echoAR, Inc. (dba "echo3D") 2018-2021.                    *
* echoAR, Inc. proprietary and confidential.                              *
*                                                                         *
* Use subject to the terms of the Terms of Service available at           *
* https://www.echo3D.co/terms, or another agreement                       *
* between echoAR, Inc. and you, your company or other organization.       *
***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System.Globalization;
using UnityEngine.Video;
using WackAMole;

public class CustomBehaviour : MonoBehaviour
{
    [HideInInspector]
    public Entry entry;

    [HideInInspector]
    public bool disableRemoteTransformations = false;
    /// <summary>
    /// EXAMPLE BEHAVIOUR
    /// Queries the database and names the object based on the result.
    /// </summary>

    public GameObject obj;
    public float hiddenHeight = -2f;
    public float visibleHeight = 1f;
    public Vector3 newPosition;
    private Vector3 initialWorldSpacePosition;
    private Quaternion initialWorldSpaceRotation;
    private Vector3 initialScale;


    void Awake()
    {
        transform.localPosition = newPosition;
    }
   
    // Use this for initialization
    void Start()
    {
        //Add RemoteTransformations script to object and set its entry
        // if (!disableRemoteTransformations)
        // {
        //     this.gameObject.AddComponent<RemoteTransformations>().entry = entry;
        // }
        RemoteTransformations();
        // Qurey additional data to get the name
        string value = "";
        if (entry.getAdditionalData() != null && entry.getAdditionalData().TryGetValue("name", out value))
        {
            // Set name
            this.gameObject.name = value;
        }


        Echo3DService.HologramStart();

        
        //search for game manager script and append gameobject to it
        //find array then do array.add and parameter is this.gameobject
       
        this.gameObject.AddComponent<Mole>();
       
    }


 
    // Update is called once per frame
    void Update()
    {
        FindMeshAndAddCollider();
        //check what state they're in (moving up/down), how many are in certain states -- adjust logic in terms of selecting new moles to put into play
        
    }
//because its void, you're not applying transformation to anything
//need to pass in gameobject to apply those positions on
//write down the flow of how game logic goes: take down states, what interactions can/can't happen at each point
//boolean to tell if gameobject is interactable or not
    //item interactable until all the way down (express through boolean)
    ///create class Mole that keeps in mind this boolean and add the component to each of the moles you bring in
    
    public void FindMeshAndAddCollider(){
        //transform.getChild
        //use a check that the getChild isnt null then call meshrenderer on the child
        //this.transform.getChild(0).gameObject..
        //call some components to adjust settings
        //MeshCollider newCollider = this.transform.getChild(0).gameObject.addComponent<MeshCollider>();
        //save it in case you have to change any of the settings
        //make sure layer collider is on will be detected by the raycast --layer masking
        //newCollider.mesh = this.transform.getChild(0).gameObject.getComponent<MeshRender>(); - in case the mesh isn't created
        if (this.gameObject.transform.childCount > 0) {
            if (this.gameObject.transform.GetChild(0).gameObject != null){
                MeshRenderer currentRenderer = this.gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
                if (currentRenderer != null) {
                    MeshCollider newCollider = this.gameObject.transform.GetChild(0).gameObject.AddComponent<MeshCollider>();
                }
            }
        }
    }

    public void RemoteTransformations(){
        initialWorldSpacePosition = (this.gameObject.transform.parent) ? this.gameObject.transform.parent.transform.position : this.gameObject.transform.position;
        initialWorldSpaceRotation = this.gameObject.transform.rotation;
        initialScale = this.gameObject.transform.localScale;

        string value = "";
        Vector3 positionOffset = Vector3.zero;
        bool applyPositionOffset = false;
        if (entry.getAdditionalData().TryGetValue("x", out value))
        {
            applyPositionOffset = true;
            positionOffset.x = float.Parse(value, CultureInfo.InvariantCulture);
        }
        if (entry.getAdditionalData().TryGetValue("y", out value))
        {
            applyPositionOffset = true;
            positionOffset.y = float.Parse(value, CultureInfo.InvariantCulture);
        }
        if (entry.getAdditionalData().TryGetValue("z", out value))
        {
            applyPositionOffset = true;
            positionOffset.z = float.Parse(value, CultureInfo.InvariantCulture);
        }
        if (applyPositionOffset)
        {
            this.gameObject.transform.localPosition = positionOffset;
        }

        // Handle spinning
        float speed = 150;
        if (entry.getAdditionalData().TryGetValue("speed", out value))
        {
            speed *= float.Parse(value, CultureInfo.InvariantCulture);
        }
        float offset = 0;
        if (entry.getAdditionalData().TryGetValue("direction", out value))
        {
            if (value.Equals("right"))
                offset += Time.time % 360 * speed;
            else
                offset -= Time.time % 360 * speed;
        }

        // Handle rotation
        Quaternion targetQuaternion = initialWorldSpaceRotation;
        float x = 0, y = 0, z = 0;
        bool applyRotation = false;
        if (entry.getAdditionalData().TryGetValue("xAngle", out value))
        {
            applyRotation = true;
            x = float.Parse(value, CultureInfo.InvariantCulture);
        }
        if (entry.getAdditionalData().TryGetValue("yAngle", out value))
        {
            applyRotation = true;
            y = float.Parse(value, CultureInfo.InvariantCulture);
        }
        if (entry.getAdditionalData().TryGetValue("zAngle", out value))
        {
            applyRotation = true;
            z = float.Parse(value, CultureInfo.InvariantCulture);
        }
        if (applyRotation || offset != 0)
        {
            this.gameObject.transform.rotation = Quaternion.Euler(x, y + offset, z);
        }

        // Handle Height and Width
        float height = (entry.getAdditionalData() != null && entry.getAdditionalData().TryGetValue("height", out value)) ? float.Parse(value) * 0.01f : 1;
        float width = (entry.getAdditionalData() != null && entry.getAdditionalData().TryGetValue("width", out value)) ? float.Parse(value) * 0.01f : 1;
        if (height != 1 || width != 1)
        {
            this.gameObject.transform.localScale = initialScale = new Vector3(width, height, height);
        }

        // Handle Scale
        float scaleFactor = 1f;
        if (entry.getAdditionalData().TryGetValue("scale", out value))
        {
            scaleFactor = float.Parse(value, CultureInfo.InvariantCulture);
            this.gameObject.transform.localScale = initialScale * scaleFactor;
        }

        // Mute
        bool mute = false;
        if (entry.getAdditionalData() != null && entry.getAdditionalData().TryGetValue("mute", out value))
        {
            mute = value.Equals("true") ? true : false;
            VideoPlayer videoPlayer = this.GetComponent<VideoPlayer>();
            for (ushort i = 0; videoPlayer != null && i < videoPlayer.controlledAudioTrackCount; ++i)
                videoPlayer.SetDirectAudioMute(i, mute);
        }

    }
}

//write down logic, all the states and events that happen in gameplay (ex: gameObject on the way up and is interactable, someone interacts, what happens> etc etc)
//because its void, you're not applying transformation to anything
//need to pass in gameobject to apply those positions on
//write down the flow of how game logic goes: take down states, what interactions can/can't happen at each point
//boolean to tell if gameobject is interactable or not
    //item interactable until all the way down (express through boolean)
    ///create class Mole that keeps in mind this boolean and add the component to each of the moles you bring in