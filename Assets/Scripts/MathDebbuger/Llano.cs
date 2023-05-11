using CustomMath;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public struct Llano
{
    internal const int size = 16;

    private CustomMath.Vector3 m_Normal;

    private float m_Distance;


    public CustomMath.Vector3 normal
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

    public Llano (CustomMath.Vector3 inNormal, CustomMath.Vector3 inPoint) 
    {
        m_Normal = CustomMath.Vector3.Normalize(inNormal);
        m_Distance = 0f - CustomMath.Vector3.Dot(m_Normal, inPoint);
    }

    public Llano(CustomMath.Vector3 inNormal, float d)
    {
        m_Normal = CustomMath.Vector3.Normalize(inNormal);
        m_Distance = d;
    }

    public Llano(CustomMath.Vector3 a, CustomMath.Vector3 b, CustomMath.Vector3 c)
    {
        m_Normal = CustomMath.Vector3.Normalize(CustomMath.Vector3.Cross(b - a, c - a));
        m_Distance = 0f - CustomMath.Vector3.Dot(m_Normal, a);
    }

    public void SetNormalAndPosition(CustomMath.Vector3 inNormal, CustomMath.Vector3 inPoint)
    {
        m_Normal = CustomMath.Vector3.Normalize(inNormal);
        m_Distance = -CustomMath.Vector3.Dot(inNormal, inPoint); 
    }

    public void Set3Points(CustomMath.Vector3 a, CustomMath.Vector3 b, CustomMath.Vector3 c)
    {
        m_Normal = CustomMath.Vector3.Normalize(CustomMath.Vector3.Cross(b - a, c - a));
        m_Distance = -CustomMath.Vector3.Dot(m_Normal, a);
    }

    public void Flip()
    {
        m_Normal = -m_Normal;
        m_Distance = -m_Distance;
    }

    public void Translate(CustomMath.Vector3 translation)
    {
        m_Distance += CustomMath.Vector3.Dot(m_Normal, translation);
    }

    public static Llano Translate(Llano llano, CustomMath.Vector3 translation) 
    {
        return new Llano(llano.m_Normal, llano.m_Distance += CustomMath.Vector3.Dot(llano.m_Normal, translation));
    }

    public CustomMath.Vector3 ClosestPointOnPlane(CustomMath.Vector3 point)
    {
        float num = CustomMath.Vector3.Dot(m_Normal, point) + m_Distance;
        return point - m_Normal * num;
    }

    public float GetDistanceToPoint(CustomMath.Vector3 point)
    {
        return CustomMath.Vector3.Dot(m_Normal, point) + m_Distance;
    }

    public bool GetSide(CustomMath.Vector3 point)
    {
        return CustomMath.Vector3.Dot(m_Normal, point) + m_Distance > 0f;
    }

    public bool SameSide(CustomMath.Vector3 inPt0, CustomMath.Vector3 inPt1)
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
