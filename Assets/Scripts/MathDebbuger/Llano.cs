using CustomMath;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public struct Llano
{
    internal const int size = 16;

    private CustomMath.Vec3 m_Normal;

    private float m_Distance;

    public Vec3 a;
    public Vec3 b;
    public Vec3 c;

    public CustomMath.Vec3 normal
    {
        get { return m_Normal; }
        set { m_Normal = value; }
    }

    public float distance
    {
        get { return m_Distance; }
        set { m_Distance = value; }
    }

    public Llano flipped => new Llano(-m_Normal, 0f - m_Distance);

    public Llano (CustomMath.Vec3 inNormal, CustomMath.Vec3 inPoint) 
    {
        m_Normal = CustomMath.Vec3.Normalize(inNormal);
        m_Distance = 0f - CustomMath.Vec3.Dot(m_Normal, inPoint);
        this.a = Vec3.Zero;
        this.b = Vec3.Zero;
        this.c = Vec3.Zero;
    }

    public Llano(CustomMath.Vec3 inNormal, float d)
    {
        m_Normal = CustomMath.Vec3.Normalize(inNormal);
        m_Distance = d;
        this.a = Vec3.Zero;
        this.b = Vec3.Zero;
        this.c = Vec3.Zero;
    }

    public Llano(CustomMath.Vec3 a, CustomMath.Vec3 b, CustomMath.Vec3 c)
    {
        m_Normal = CustomMath.Vec3.Normalize(CustomMath.Vec3.Cross(b - a, c - a));
        m_Distance = 0f - CustomMath.Vec3.Dot(m_Normal, a);
        this.a = a;
        this.b = b;
        this.c = c;
    }

    public void SetNormalAndPosition(CustomMath.Vec3 inNormal, CustomMath.Vec3 inPoint)
    {
        m_Normal = CustomMath.Vec3.Normalize(inNormal);
        m_Distance = -CustomMath.Vec3.Dot(inNormal, inPoint); 
    }

    public void Set3Points(CustomMath.Vec3 a, CustomMath.Vec3 b, CustomMath.Vec3 c)
    {
        m_Normal = CustomMath.Vec3.Normalize(CustomMath.Vec3.Cross(b - a, c - a));
        m_Distance = -CustomMath.Vec3.Dot(m_Normal, a);

    }

    public void Flip()
    {
        m_Normal = -m_Normal;
        m_Distance = -m_Distance;
    }

    public void Translate(CustomMath.Vec3 translation)
    {
        m_Distance += CustomMath.Vec3.Dot(m_Normal, translation);
    }

    public static Llano Translate(Llano llano, CustomMath.Vec3 translation) 
    {
        return new Llano(llano.m_Normal, llano.m_Distance += CustomMath.Vec3.Dot(llano.m_Normal, translation));
    }

    public CustomMath.Vec3 ClosestPointOnPlane(CustomMath.Vec3 point)
    {
        float num = CustomMath.Vec3.Dot(m_Normal, point) + m_Distance;
        return point - m_Normal * num;
    }

    public float GetDistanceToPoint(CustomMath.Vec3 point)
    {
        return CustomMath.Vec3.Dot(m_Normal, point) + m_Distance;
    }

    public bool GetSide(CustomMath.Vec3 point)
    {
        return CustomMath.Vec3.Dot(m_Normal, point) + m_Distance > 0f;
    }

    public bool SameSide(CustomMath.Vec3 inPt0, CustomMath.Vec3 inPt1)
    {
        float distanceToPoint = GetDistanceToPoint(inPt0);
        float distanceToPoint2 = GetDistanceToPoint(inPt1);
        return (distanceToPoint > 0f && distanceToPoint2 > 0f) || (distanceToPoint <= 0f && distanceToPoint2 <= 0f);
    }
    /*
    public bool Raycast(Ray ray, out float enter)
    {
      float num = CustomMath.Vector3.Dot(ray.direction, m_Normal);
      float num2 = 0f - CustomMath.Vector3.Dot(ray.origin, m_Normal) - m_Distance;
      if (Mathf.Approximately(num, 0f))
      {
          enter = 0f;
          return false;
      }
      
      enter = num2 / num;
      return enter > 0f;
    }
    */
}
