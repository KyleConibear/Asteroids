using UnityEngine;

namespace KyleConibear
{
	public static class Math
	{
		/// <summary>
		/// Determine if an object is on the left or right based on there relative angle
		/// </summary>
		/// <returns>0=left, 1=right</returns>
		public static byte BinaryPositionFromRelativeAngle(Vector3 target)
		{
			Vector3 perp = Vector3.Cross(Vector3.forward, target);
			float direction = Vector3.Dot(perp, Vector3.up);

			if (direction > 0f)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		/// <summary>
		/// Check where a object is locations relative to origin
		/// </summary>
		/// <param name="origin">The position in which to check if an object is on either side of</param>
		/// <param name="target">The position your checking against the origin</param>
		/// <returns>
		/// [0]=x, [1]=y, [2]=z axis
		/// 0=left, 1=right
		/// </returns>
		public static byte[] BinaryPositionFromOrigin(Vector3 origin, Vector3 target)
		{
			byte[] binaryPos = new byte[3];

			if (target.x < origin.x)
			{
				binaryPos[0] = 0;
			}
			else if (target.x > origin.x)
			{
				binaryPos[0] = 1;
			}

			if (target.y < origin.y)
			{
				binaryPos[1] = 0;
			}
			else if (target.y > origin.y)
			{
				binaryPos[1] = 1;
			}

			if (target.z < origin.z)
			{
				binaryPos[2] = 0;
			}
			else if (target.z > origin.z)
			{
				binaryPos[2] = 1;
			}

			return binaryPos;
		}
	}
}