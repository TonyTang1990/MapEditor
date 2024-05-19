/*
 * Description:             Collision2DUtilities.cs
 * Author:                  TangHuan
 * Create Date:             2021/03/23
 */

using System;
using UnityEngine;

namespace TH.Module.Collision2D
{
    /// <summary>
    /// 2D碰撞单例工具类
    /// </summary>
    public static class Collision2DUtilities
    {
        #region 圆形相交部分
        /// <summary>
        /// Vector2临时变量(用于优化new Vector2)
        /// </summary>
        private static Vector3 Vector2Temp;

        /// <summary>
        /// Vector3临时变量(用于优化new Vector3)
        /// </summary>
        private static Vector3 Vector3Temp;

        /// <summary>
        /// 判断圆形与点之间的交叉检测
        /// </summary>
        /// <param name="circle1"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool CircleAndPointIntersection(Circle2D circle1, Vector2 point)
        {
            return (circle1.Center - point).sqrMagnitude < circle1.Radius * circle1.Radius;
        }

        /// <summary>
        /// 判断圆形与圆形之间的交叉检测
        /// </summary>
        /// <param name="circle1"></param>
        /// <param name="circle2"></param>
        /// <returns></returns>
        public static bool CircleAndCircleIntersection(Circle2D circle1, Circle2D circle2)
        {
            return (circle1.Center - circle2.Center).sqrMagnitude < (circle1.Radius + circle2.Radius) * (circle1.Radius + circle2.Radius);
        }

        /// <summary>
        /// 判断圆形与胶囊体相交
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="capsule"></param>
        /// <returns></returns>
        public static bool CircleAndCapsuleIntersection(Circle2D circle, Capsule2D capsule)
        {
            float sqrdistance = SqrDistanceBetweenSegmentAndPoint(capsule.StartPoint, capsule.PointLine, circle.Center);
            return sqrdistance < (circle.Radius + capsule.Radius) * (circle.Radius + capsule.Radius);
        }

        /// <summary>
        /// 判断圆形与扇形相交
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="sector"></param>
        /// <returns></returns>
        public static bool CircleAndSectorIntersection(Circle2D circle, Sector2D sector)
        {
            Vector2 tempDistance = circle.Center - sector.StartPoint;
            float halfAngle = Mathf.Deg2Rad * sector.Angle / 2;
            // 判定扇形起点和圆心距离是否小于扇形半径+圆形半径
            if (tempDistance.sqrMagnitude < (sector.Radius + circle.Radius) * (sector.Radius + circle.Radius))
            {
                // 判定圆形是否在扇形角度内
                if (Vector3.Angle(tempDistance, sector.Direction) < sector.Angle / 2)
                {
                    return true;
                }
                else
                {
                    // 利用扇形的对称性，将圆心位置映射到一侧
                    Vector2 targetInsectorAxis = new Vector2(Vector2.Dot(tempDistance, sector.Direction), Mathf.Abs(Vector2.Dot(tempDistance, new Vector2(-sector.Direction.y, sector.Direction.x))));
                    // 扇形的一条边
                    Vector2 directionInSectorAxis = sector.Radius * new Vector2(Mathf.Cos(halfAngle), Mathf.Sin(halfAngle));
                    // 通过判定映射后的原因与扇形一条边的距离是否小于圆形半径判定相交
                    return SqrDistanceBetweenSegmentAndPoint(Vector2.zero, directionInSectorAxis, targetInsectorAxis) <= circle.Radius * circle.Radius;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断圆形与多边形相交
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static bool CircleAndPolygonIntersection(Circle2D circle, Polygon2D polygon)
        {
            // 判定圆心是否在多边形内
            if (BetterPointInConvexPolygon(polygon, circle.Center))
            {
                return true;
            }
            // 圆心不在多边形内，则判定圆心与每条边的距离是否小于半径
            Vector2 circleCenter = circle.Center;
            float sqrR = circle.Radius * circle.Radius;
            var vertexes = polygon.Vertexes;
            int polygonVertexsNumber = polygon.Vertexes.Length;
            Vector2 edge;
            int index;
            for (int i = 0, length = polygonVertexsNumber; i < length; i++)
            {
                index = (i + 1) % polygonVertexsNumber;
                edge.x = vertexes[index].x - vertexes[i].x;
                edge.y = vertexes[index].y - vertexes[i].y;
                // 判定圆心到单条边的距离是否小于半径
                if (SqrDistanceBetweenSegmentAndPoint(vertexes[i], edge, circleCenter) < sqrR)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断圆形与AABB相交
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="aabb"></param>
        /// <returns></returns>
        public static bool CircleAndAABBIntersection(Circle2D circle, AABB2D aabb)
        {
            // 以AABB中心为圆心
            Vector2 v = Vector2.Max(circle.Center - aabb.Center, -(circle.Center - aabb.Center));
            // 把圆心坐标偏移AABB大小
            Vector2 u = Vector2.Max(v - aabb.Extents / 2, Vector2.zero);
            // 判定圆心距离是否还大于圆形半径判定相交
            return u.sqrMagnitude < circle.Radius * circle.Radius;
        }

        /// <summary>
        /// 判定圆形和OBB相交检测
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="obb"></param>
        /// <returns></returns>
        public static bool CircleAndOBBIntersection(Circle2D circle, OBB2D obb)
        {
            // 以OBB中心为坐标原点
            Vector2 point = circle.Center - obb.Center;
            Vector3Temp.x = point.x;
            Vector3Temp.y = 0;
            Vector3Temp.z = point.y;
            // 将圆心通过反向旋转转到OBB坐标系
            Vector3 point2 = Quaternion.AngleAxis(-obb.Angle, Vector3.up) * Vector3Temp;
            point.x = point2.x;
            point.y = point2.z;
            // 将旋转后的圆心位置和OBB做AABB方式的相交判定
            Vector2 v = Vector2.Max(point, -point);
            // 把圆心坐标偏移OBB大小
            Vector2 u = Vector2.Max(v - obb.Extents / 2, Vector2.zero);
            // 判定圆心距离是否还大于圆形半径判定相交
            return u.sqrMagnitude < circle.Radius * circle.Radius;
        }

        /// <summary>
        /// 判定AABB和AABB相交检测
        /// </summary>
        /// <param name="aabb1"></param>
        /// <param name="aabb2></param>
        /// <returns></returns>
        public static bool AABBAndAABBIntersection(AABB2D aabb1, AABB2D aabb2)
        {
            // 两个AABB原点的位置偏移
            Vector2 offset = aabb2.Center - aabb1.Center;
            offset = Vector2.Max(offset, -offset);
            // 判定偏移是否小于两者宽/2之和和两者长/2之和
            return offset.x <= (aabb1.Extents.x / 2 + aabb2.Extents.x / 2) && offset.y <= (aabb1.Extents.y / 2 + aabb2.Extents.y / 2);
        }
        #endregion

        #region 点相交部分
        /// <summary>
        /// 点是否在圆形内
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool PointInCircle(Circle2D circle, Vector2 point)
        {
            return CircleAndPointIntersection(circle, point);
        }

        /// <summary>
        /// 点是否在矩形(AABB)内
        /// </summary>
        /// <param name="aabb"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool PointInAABB(AABB2D aabb, Vector2 point)
        {
            // 点和AABB原点的位置偏移(等价于将点移动到AABB坐标系)
            Vector2 offset = aabb.Center - point;
            offset = Vector2.Max(offset, -offset);
            // 判定偏移是否小于AABB宽/2和AABB长/2
            return offset.x <= aabb.Extents.x / 2 && offset.y <= aabb.Extents.y / 2;
        }

        /// <summary>
        /// 点是否在多边形内(内角和法)
        /// </summary>
        /// <param name="polygon">多边形</param>
        /// <param name="point">点</param>
        /// <returns></returns>
        public static bool PointInPolygon(Polygon2D polygon, Vector2 point)
        {
            if(!polygon.IsValide())
            {
                return false;
            }
            Vector2[] polygonVertexes = polygon.Vertexes;
            int polygonVertexsNumber = polygonVertexes.Length;
            // 点与所有多边形顶点的连线向量
            Vector2[] pointLines = new Vector2[polygonVertexsNumber];
            for(int i = 0, length = polygonVertexsNumber; i < length; i++)
            {
                pointLines[i] = polygonVertexes[i] - point;
            }
            // 计算所有多边形顶点连线之间的夹角总和
            float totalAngle = Vector2.SignedAngle(pointLines[polygonVertexsNumber - 1], pointLines[0]);
            for(int i = 0, length = polygonVertexsNumber - 1; i < length; i++)
            {
                totalAngle += Vector2.SignedAngle(pointLines[i], pointLines[i + 1]);
            }
            if (Mathf.Abs(Mathf.Abs(totalAngle) - 360f) < 0.1f)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 点是否在凸边形内(叉积(点线)法)
        /// Note:
        /// 1. 确保是凸多边形
        /// 2. 确保凸多边形顶点是顺时针
        /// </summary>
        /// <param name="polygon">凸多边形</param>
        /// <param name="point">点</param>
        /// <returns></returns>
        public static bool PointInConvexPolygon(Polygon2D polygon, Vector2 point)
        {
            if (!polygon.IsValide())
            {
                return false;
            }
            Vector2[] polygonVertexes = polygon.Vertexes;
            int polygonVertexsNumber = polygonVertexes.Length;
            Vector3 pointLine;
            Vector3 polygonEdge;
            int index;
            // 这里采用顶点顺时针判定
            for (int i = 0, length = polygonVertexsNumber; i < length; i++)
            {
                pointLine = new Vector3(point.x - polygonVertexes[i].x, 0, point.y - polygonVertexes[i].y);
                index = (i + 1) % polygonVertexsNumber;
                polygonEdge = new Vector3(polygonVertexes[index].x - polygonVertexes[i].x, 0, polygonVertexes[index].y - polygonVertexes[i].y);
                // 计算多边形顶点的连线向量和边的左右关系
                // Vector3.Cross(A, B).y > 0 表示A在B的左侧
                // Vector3.Cross(A, B).y < 0 表示A在B的右侧
                // Vector3.Cross(A, B).y == 0 表示A和B平行
                if (Vector3.Cross(pointLine, polygonEdge).y >= 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 点是否在凸边形内(叉积(点线)法--不构建Vector3纯数学运算)
        /// Note:
        /// 1. 确保是凸多边形
        /// 2. 确保凸多边形顶点是顺时针
        /// </summary>
        /// <param name="polygon">凸多边形</param>
        /// <param name="point">点</param>
        /// <returns></returns>
        public static bool BetterPointInConvexPolygon(Polygon2D polygon, Vector2 point)
        {
            if (!polygon.IsValide())
            {
                return false;
            }
            return BetterPointInVertexes(polygon.Vertexes, point);
        }

        /// <summary>
        /// 点是否在顶点内(叉积(点线)法--不构建Vector3纯数学运算)
        /// Note:
        /// 1. 确保是凸多边形
        /// 2. 确保凸多边形顶点是顺时针
        /// </summary>
        /// <param name="polygon">凸多边形</param>
        /// <param name="point">点</param>
        /// <returns></returns>
        public static bool BetterPointInVertexes(Vector2[] vertexes, Vector2 point)
        {
            if (vertexes.Length < 3)
            {
                return false;
            }
            int polygonVertexsNumber = vertexes.Length;
            int index;
            float lineX;
            float lineZ;
            float edgeX;
            float edgeZ;
            // 把2D向量当做XZ平面的3D向量来处理
            // 不构建Vector3的边，直接利用叉乘公式计算叉乘后的Y值
            // 然后通过Y值来判定两个2D向量的方向
            // 这里采用顶点顺时针判定
            for (int i = 0, length = polygonVertexsNumber; i < length; i++)
            {
                lineX = point.x - vertexes[i].x;
                lineZ = point.y - vertexes[i].y;
                index = (i + 1) % polygonVertexsNumber;
                edgeX = vertexes[index].x - vertexes[i].x;
                edgeZ = vertexes[index].y - vertexes[i].y;
                // 利用叉乘原始公式进行计算判定两个向量的方向
                // Vector3.Cross(A, B).y > 0 表示A在B的左侧
                // Vector3.Cross(A, B).y < 0 表示A在B的右侧
                // Vector3.Cross(A, B).y == 0 表示A和B平行
                if ((lineZ * edgeX - lineX * edgeZ) > 0)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region 公共部分
        /// <summary>
        /// 线段与点的最短距离
        /// </summary>
        /// <param name="startpoint">线段起点</param>
        /// <param name="line">线段向量</param>
        /// <param name="targetpoint">求解点</param>
        /// <returns></returns>
        public static float SqrDistanceBetweenSegmentAndPoint(Vector2 startpoint, Vector2 line, Vector2 targetpoint)
        {
            float t = Vector2.Dot(targetpoint - startpoint, line) / line.sqrMagnitude;
            return (targetpoint - (startpoint + Mathf.Clamp01(t) * line)).sqrMagnitude;
        }
        #endregion
    }
}