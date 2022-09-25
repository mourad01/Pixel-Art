/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件由会员免费分享，如果商用，请务必联系原著购买授权！

daily assets update for try.

U should buy a license from author if u use it in your project!
*/

using System;
using UnityEngine;

[Serializable]
public class SerializableVector3
{
	public float X { get; set; }

	public float Y { get; set; }

	public float Z { get; set; }

	public SerializableVector3(Vector3 vec)
	{
		this.X = vec.x;
		this.Y = vec.y;
		this.Z = vec.z;
	}

	public SerializableVector3(float x, float y, float z)
	{
		this.X = x;
		this.Y = y;
		this.Z = z;
	}

	public Vector3 ToVector3()
	{
		return new Vector3(this.X, this.Y, this.Z);
	}
}


