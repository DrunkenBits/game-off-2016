using UnityEngine;
using System.Collections;

public class TrackedCameraController : MonoBehaviour 
{
	public const int TARGET_WIDTH = 1280;
	public const int TARGET_HEIGHT = 720;
	
	public Transform target;
	public Vector3? focusPoint = null;
	public float dampen = 1.5f;
	public bool lockY = false;
	public CameraRail rail;
	
	private float orthoFactor;
	private Vector2 halfVector;
	
	private Vector3 velocity;
	private Vector3 velocityRaw;
	private Vector3 ds;

	void Start () 
	{
		halfVector.y = GetComponent<Camera>().orthographicSize;
		orthoFactor = halfVector.y / TARGET_HEIGHT;
		halfVector.x = TARGET_WIDTH * orthoFactor;

		if (!GetComponent<Camera>().orthographic) {
			GetComponent<Camera>().transparencySortMode = TransparencySortMode.Orthographic;
		}
	}

	void Update () 
	{
		Vector3 tp = getTarget();
		Vector3 cp = getCurrent();
		
		//Calculate the distance between the current position/zoom and the target's position/zoom
		ds = tp - cp;
		velocity = ds * (1.0f / dampen) * Time.deltaTime;
		
		if (lockY)
			velocity.y = 0;
		
		Rect nb = getNewBounds(velocity);
		
		//Check if the future camera bounds are legal (within the specified bounds).
		//If they are not, try to compensate with available space on the same axis.
		//If the camera frame contains the bounds on one of the axis all we can do
		//is stop the camera from panning/zooming because no compensation is possible.
		
		//TODO: Slow deceleration near edges!!
		
		if ((nb.xMin < rail.cameraBounds.xMin && nb.xMax > rail.cameraBounds.xMax) ||
			(nb.yMin < rail.cameraBounds.yMin && nb.yMin > rail.cameraBounds.yMin))
		{
			velocity = Vector3.zero;
		}
		else
		{
			if (nb.xMin < rail.cameraBounds.xMin)
				velocity.x += rail.cameraBounds.xMin - nb.xMin;
			else if (nb.xMax > rail.cameraBounds.xMax)
				velocity.x -= nb.xMax - rail.cameraBounds.xMax;
			
			if (nb.yMin < rail.cameraBounds.yMin)
				velocity.y += rail.cameraBounds.yMin - nb.yMin;
			else if (nb.yMax > rail.cameraBounds.yMax)
				velocity.y -= nb.yMax - rail.cameraBounds.yMax;
		}
		
		velocityRaw = velocity * (1.0f / Time.deltaTime);

		Vector3 increment = new Vector3(velocity.x, velocity.y, 0);

		GetComponent<Camera>().orthographicSize += velocity.z;

		if (!GetComponent<Camera>().orthographic) {
			increment.z = getPerspDepthFromOrtho(
				GetComponent<Camera>().orthographicSize,
				0
			);

			increment.z -= GetComponent<Camera>().transform.position.z;
		}

		transform.position += increment;
	}

	private float getPerspDepthFromOrtho(float ortographicSize, float focalDepth = 0)
	{
		Camera camera = GetComponent<Camera>();

		float fovRad = camera.fieldOfView * 0.5f * Mathf.Deg2Rad;
		float distance = (ortographicSize / Mathf.Sin(fovRad)) * Mathf.Cos(fovRad);

		return focalDepth - distance;
	}
	
	private Vector3 getTarget()
	{
		Vector3 tp = new Vector3();
		
		if (focusPoint != null)
		{
			tp = focusPoint.GetValueOrDefault();
		}
		else if (target != null)
		{
			tp = target.position;
			
			//Assume the zoom to be the standard value
			tp.z = halfVector.y;
		
			//If the camera is on a rail, get the rail position for the
			//target's x coordinate.
			if (rail != null)
			{
				Vector3? nearest = rail.GetPointAt(target.position);
				
				//If there is a point on the rail for the target's x coordinate
				//than we have our new camera target point
				if (nearest != null)
					tp = nearest.GetValueOrDefault();
			}
		}
		
		return tp;
	}
	
	private Vector3 getCurrent()
	{
		return new Vector3(transform.position.x, transform.position.y, GetComponent<Camera>().orthographicSize);
	}
	
	private Rect getNewBounds(Vector3 velocity)
	{
		float zoomFactor = (GetComponent<Camera>().orthographicSize + velocity.z) / halfVector.y;
		Vector3 futurePos = transform.position + new Vector3(velocity.x, velocity.y, 0);
		
		Rect newBounds = new Rect(
			futurePos.x - (halfVector.x * zoomFactor),
			futurePos.y - (halfVector.y * zoomFactor),
			halfVector.x * 2 * zoomFactor,
			halfVector.y * 2 * zoomFactor
		);
		
		return newBounds;
	}
	
	public Vector3 GetVelocity()
	{
		return velocityRaw;
	}
}
