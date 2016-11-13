using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CameraRail : MonoBehaviour 
{
	public Vector3 position = new Vector3();
	public List<Vector3> railPoints = new List<Vector3>();
	public Rect cameraBounds;
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	
	public List<Vector3> GetRailPoints()
	{
		return new List<Vector3>(railPoints);
	}
	
    public Vector3? GetPointAt(Vector3 point)
    {
		Vector3? a = null;
		Vector3? b = null;
		
    	foreach(Vector3 p in railPoints)
    	{
        	if (p.x <= point.x)
        	{
        		a = p;
        	}
        	else
        	{
        		b = p;
        		break;
        	}
    	}

    	if (a != null && b != null)
    	{
			Vector3 av = a.GetValueOrDefault();
			Vector3 bv = b.GetValueOrDefault();
			
            Vector3 segVec = bv - av;
    		float percent = ((point.x - av.x) / (bv.x - av.x));

            segVec *= percent;
            segVec += av;
			
            return segVec;
    	}

    	return null;
    }
	
	void OnDrawGizmos () 
	{
        Gizmos.color = Color.red;
        DrawRect(cameraBounds);

        for (int i = 0; i < railPoints.Count; i++)
        {
            Vector3[] zoomRange = getRailZoomRange(i);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(zoomRange[0], zoomRange[1]);
        }

		if (railPoints.Count >= 2)
		{	
			for(int i = 1; i < railPoints.Count; i++) 
			{
			    Gizmos.color = Color.white;

				Gizmos.DrawWireSphere(railPoints[i-1], 0.5f);
				Gizmos.DrawLine(railPoints[i-1], railPoints[i]);

                Vector3[] zoomRangeA = getRailZoomRange(i - 1);
                Vector3[] zoomRangeB = getRailZoomRange(i);

                Gizmos.color = Color.green;

                Gizmos.DrawLine(zoomRangeA[0], zoomRangeB[0]);
                Gizmos.DrawLine(zoomRangeA[1], zoomRangeB[1]);
			}
			
			Gizmos.DrawWireSphere(railPoints[railPoints.Count-1], 0.5f);
		}
    }

    public Vector3[] getRailZoomRange(int i)
    {
        i = Mathf.Max(0, Mathf.Min(i, railPoints.Count - 1));

        return new Vector3[] {
            new Vector3(railPoints[i].x, railPoints[i].y - railPoints[i].z, 0),
            new Vector3(railPoints[i].x, railPoints[i].y + railPoints[i].z, 0)
        };
    }
			
	protected void DrawRect(Rect r)
	{
		Gizmos.DrawLine(new Vector3(r.x, r.y, 0), new Vector3(r.x + r.width, r.y, 0));	
		Gizmos.DrawLine(new Vector3(r.x + r.width, r.y, 0), new Vector3(r.x + r.width, r.y + r.height, 0));	
		Gizmos.DrawLine(new Vector3(r.x + r.width, r.y + r.height, 0), new Vector3(r.x, r.y + r.height, 0));
		Gizmos.DrawLine(new Vector3(r.x, r.y + r.height, 0), new Vector3(r.x, r.y, 0));
	}
}
