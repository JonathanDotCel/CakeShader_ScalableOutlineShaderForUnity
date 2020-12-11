
// MIT license: https://choosealicense.com/licenses/mit/ <-- I pick this one
//
// Stores modified vert normals in the colour data, 
// so we can get consistently scaled mesh outlines.
// 

using System.Collections.Generic;
using UnityEngine;

public class CakeMeshProcessor : MonoBehaviour
{
    
    [Header("Debug (ReadOnly)")]
    public Mesh originalMesh;
    
    [ContextMenu("ProcessMesh")]
    public void ProcessMesh(){

        // I wouldn't normally advocate this, but since
        // it's a proof of concept, let's skip the error
        // checking for the sake of extreme clarity here.
        // Future employers: look away!
        
        MeshFilter mf = GetComponent<MeshFilter>();
        
        if ( originalMesh == null )
            originalMesh = mf.sharedMesh;

        Mesh newMesh = (Mesh)Instantiate( originalMesh );
        
        newMesh.name += "_baked";
        
        List<Color> vertColors = new List<Color>( newMesh.vertexCount );
        
        // cached so it's not cloning the array on every iteration        
        Vector3[] verts = newMesh.vertices;
        Vector3[] norms = newMesh.normals;

        for( int i = 0; i < newMesh.vertexCount; i++ ){
            
            // Faces associated with this vert
            List<Vector3> activeFaces = new List<Vector3>();

            // If another vert shares the same space as this one
            // then we'll average in that normal
            for( int a = 0; a < newMesh.vertexCount; a++ ){
                if ( a == i ) continue;
                if ( verts[i] == verts[a] )
                    activeFaces.Add( norms[a] );
            }

            activeFaces.Add( norms[i] );

            // average the faces (normals)
            Vector3 vertColor = Vector3.zero;
            for( int n = 0; n < activeFaces.Count; n++ ){
                vertColor += activeFaces[n];
            }
            vertColor.Normalize();
            
            // Even if it's orphaned, etc
            vertColors.Add( V3ToColor( vertColor ) );
            
        }

        newMesh.SetColors( vertColors );
        
        mf.sharedMesh = newMesh;
        
    }

    // Use this method if you're targeting a much older
    // version of unity. Should be fine though as they
    // back ported the float support quite far back.
    byte Remap( float inVal ){
        return (byte)((( inVal * 0.5f ) + 0.5f)*255);
    }

    Color V3ToColor( Vector3 inVector ){
        return new Color( inVector.x, inVector.y, inVector.z, 0 );
        //return new Color32( Remap(inVector.x), Remap(inVector.y), Remap(inVector.z), 0 );
    }
    
    
    [ContextMenu("RestoreOriginalMesh")]
    public void RestoreTheThing(){
        
        if ( originalMesh != null )
            GetComponent<MeshFilter>().sharedMesh = originalMesh;

    }


}
