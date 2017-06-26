using System;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

/// <summary>
/// Represents extender for different classes
/// </summary>
public static class UnityExtension
{
    #region [Common]

    /// <summary>
    /// Gets a value indicating whether the unity object is destroyed. Relies on Unity's special overload of '==' operator for <see cref="Object"/>.
    /// </summary>
    /// <param name="unityObject"> Unity object to check for destruction. </param>
    public static bool IsDestroyed(this Object unityObject)
    {
        return unityObject == null;
    }

    #endregion

    #region [Transform component extensions]

    /// <summary>
    /// Recursively copy all transform values and paste it to another transform by name.
    /// </summary>
    public static void CopyByNames(this Transform source, Transform destination)
    {
        var transforms = new Dictionary<string, Transform>();
        Copy(source, transforms, string.Empty);
        Paste(destination, transforms, string.Empty);
    }

    /// <summary>
    /// Resets the state of the transform component (either to local or to global originals).
    /// </summary>
    /// <param name="transform"> Transform to reset. </param>
    /// <param name="local"> Value indicating whether local or global values should be reset. </param>
    public static void Reset(this Transform transform, bool local)
    {
        if (local)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
        else
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// Gets all first-generation children (i.e. straight transform inheritors) of specified transform.
    /// </summary>
    /// <param name="transform"> Transform, whose children is desired to get. </param>
    public static Transform[] GetFirstGenerationChildren(this Transform transform)
    {
        var children = new List<Transform>();

        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                children.Add(child);
            }
        }

        return children.ToArray();
    }

    /// <summary>
    /// Gets all first-generation children (i.e. straight transform inheritors) of specified transform.
    /// </summary>
    /// <param name="transform"> Transform, whose children is desired to get. </param>
    /// <param name="includeInactive"> Value indicating whether inactive children should be inspected for component. </param>
    public static List<T> GetComponentsInFirstGenerationChildren<T>(this Transform transform, bool includeInactive)
    {
        var children = transform.GetFirstGenerationChildren();
        var components = new List<T>(children.Length);

        for (var i = 0; i < children.Length; i++)
        {
            var child = children[i];
            if (child.gameObject.activeSelf)
            {
                var component = child.GetComponent<T>();
                if (component != null)
                {
                    components.Add(component);
                }
            }
        }

        return components;
    }

    /// <summary>
    /// Gets the first found component among all children.
    /// </summary>
    /// <typeparam name="T"> Type of the component to get. </typeparam>
    /// <param name="trandform"> Object whose children are to be inspected for component. </param>
    public static T GetFirstChildComponent<T>(this Transform trandform) where T : Component
    {
        for (var i = 0; i < trandform.childCount; i++)
        {
            var child = trandform.GetChild(i);
            var component = child.GetComponent<T>();

            if (component)
            {
                return component;
            }
        }

        return null;
    }

    /// <summary>
    /// Destroys all children of specified transform
    /// </summary>
    public static void DestroyChildren(this Transform transform)
    {
        var children = transform.GetFirstGenerationChildren();

        foreach (var child in children)
        {
            child.parent = null;
            Object.Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Sets Layer Recursively.
    /// </summary>
    public static void SetLayerRecursively(this Transform transform, int layer)
    {
        transform.gameObject.layer = layer;
        foreach (Transform t in transform)
        {
            t.gameObject.layer = layer;
            if (t.childCount > 0)
            {
                SetLayerRecursively(t, layer);
            }
        }
    }

    /// <summary>
    /// Sets the global rotation and position of transform to correspond to the global rotation and position of specified transform.
    /// </summary>
    /// <param name="transform"> Transform whose pivot need to change. </param>
    /// <param name="globlaPivot"> Transform representing new pivot. </param>
    public static void SetGlobalPivot(this Transform transform, Transform globlaPivot)
    {
        transform.position = globlaPivot.position;
        transform.rotation = globlaPivot.rotation;
    }

    /// <summary>
    /// Sets the uniform local scale.
    /// </summary>
    /// <param name="transform">The transform.</param>
    /// <param name="scale">The scale.</param>
    public static void SetUniformLocalScale(this Transform transform, float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    /// <summary>
    /// Gets the parent of the transform, which doesn't have any parent itself.
    /// </summary>
    /// <param name="transform"> Transform for which is desired to get the most root parent. </param>
    public static Transform GetMostRootParent(this Transform transform)
    {
        var transformParent = transform.parent;

        if (transformParent)
        {
            return GetMostRootParentWithSelf(transformParent);
        }

        return null;
    }

    /// <summary>
    /// Destroys the first found component of specified type from the game object (if any was found).
    /// </summary>
    /// <typeparam name="TComponent"> Type of the component to destroy from the game object. </typeparam>
    /// <param name="gameObject"> Game object which is desired to get rid of component. </param>
    public static void DestroyComponent<TComponent>(this GameObject gameObject) where TComponent : Component
    {
        var component = gameObject.GetComponent<TComponent>();
        if (component)
        {
            Object.Destroy(component);
        }
    }

    #endregion

    #region [Animation]

    /// <summary>
    /// Gets the index of the animator's layer with specified name.
    /// </summary>
    /// <param name="animator"> Animator, whose layer's index is desired to figure out. </param>
    /// <param name="name"> Expected layer's name of the animator. </param>
    /// <returns> Index of the layer with specified name. If there is no layer with specified name - -1 is returned. </returns>
    public static int GetLayerIndex(this Animator animator, string name)
    {
        var layersCount = animator.layerCount;

        for (var i = 0; i < layersCount; i++)
        {
            var layerName = animator.GetLayerName(i);

            if (layerName == name)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Tells if the animator state is currently empty on the specified layer.
    /// </summary>
    /// <param name="animator"> Reference to the animator, for which is desired to check layer's state. </param>
    /// <param name="layerIndex"> Index of the layer, for which is desired to check current state. </param>
    /// <param name="emptyStateHash"> Name of the state, which is to be treated as 'empty' one. </param>
    /// <returns> Tells if current animator state on the specified layer is the one, which has specified name. </returns>
    public static bool IsLayerStateEmpty(this Animator animator, int layerIndex, int emptyStateHash)
    {
        var currentAction = animator.GetCurrentAnimatorStateInfo(layerIndex);

        return currentAction.fullPathHash == emptyStateHash;
    }

    /// <summary>
    /// Samples animation at the specified normalized time of the animation state.
    /// </summary>
    /// <param name="animation"> Animation to sample animation for. </param>
    /// <param name="animName"> Name of the animation clip to sample the animation for. </param>
    /// <param name="normalizedTime"> Normalized time to sample animation at. </param>
    public static void Sample(this Animation animation, string animName, float normalizedTime)
    {
        var state = animation[animName];

        state.speed = 0;
        animation.Play(animName);
        state.enabled = true;
        state.normalizedTime = normalizedTime;
        animation.Sample();
        animation.Stop(animName);
        state.speed = 1;
    }

    /// <summary>
    /// Samples animation at the specified normalized time of the animation state of default animation.
    /// </summary>
    /// <param name="animation"> Animation to sample animation for. </param>
    /// <param name="normalizedTime"> Normalized time to sample animation at. </param>
    public static void Sample(this Animation animation, float normalizedTime)
    {
        var animName = animation.clip.name;
        animation.Sample(animName, normalizedTime);
    }

    #endregion

    #region [Camera]

    /// <summary>
    /// Gets the point in space, which is rendered in the same screen position for target camera, as the source point for the source camera.
    /// </summary>
    /// <param name="sourceCamera"> Source camera which defines the position on screen of the rendered point. </param>
    /// <param name="to"> Camera to project the point on. </param>
    /// <param name="sourcePoint"> Point in space to project. </param>
    public static Vector3 ProjectPointTo(this Camera sourceCamera, Camera to, Vector3 sourcePoint)
    {
        var sourceCameraPointScreenPosition = sourceCamera.WorldToScreenPoint(sourcePoint);
        return to.ScreenToWorldPoint(sourceCameraPointScreenPosition);
    }

    #endregion

    #region [Vector extensions]

    /// <summary>
    /// Checks if vector has NaN or infinite components.
    /// </summary>
    /// <param name="vector"> Vector to validate. </param>
    /// <returns> true - if all components of vector are real numbers, otherwise - false. </returns>
    public static bool IsValid(this Vector3 vector)
    {
        var hasInfiniteComponent = float.IsInfinity(vector.x) || float.IsInfinity(vector.y) || float.IsInfinity(vector.z);
        var hasNaNComponent = float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z);

        return !(hasNaNComponent || hasInfiniteComponent);
    }

    /// <summary>
    /// Gets a new vector from the existent by changing of its x-coordinate by specified delta.
    /// </summary>
    /// <param name="vector"> Vector to change. </param>
    /// <param name="xDelta"> Delta of the x-coordinate. </param>
    public static Vector3 AddX(this Vector3 vector, float xDelta)
    {
        return new Vector3(vector.x + xDelta, vector.y, vector.z);
    }

    /// <summary>
    /// Gets a new vector from the existent by changing of its y-coordinate by specified delta.
    /// </summary>
    /// <param name="vector"> Vector to change. </param>
    /// <param name="yDelta"> Delta of the y-coordinate. </param>
    public static Vector3 AddY(this Vector3 vector, float yDelta)
    {
        return new Vector3(vector.x, vector.y + yDelta, vector.z);
    }

    /// <summary>
    /// Gets a new vector from the existent by changing of its z-coordinate by specified delta.
    /// </summary>
    /// <param name="vector"> Vector to change. </param>
    /// <param name="zDelta"> Delta of the z-coordinate. </param>
    public static Vector3 AddZ(this Vector3 vector, float zDelta)
    {
        return new Vector3(vector.x, vector.y, vector.z + zDelta);
    }

    /// <summary>
    /// Gets non-orthogonal projection of a vector on another vector along the specified direction.
    /// </summary>
    /// <param name="vector"> Vector to get projection of. </param>
    /// <param name="onVector"> Target vector to project on. </param>
    /// <param name="alongDirection"> Direction to project along. </param>
    public static Vector2 Project(this Vector2 vector, Vector2 onVector, Vector2 alongDirection)
    {
        if (IsCollinear(vector, onVector))
        {
            return vector;
        }

        if (IsCollinear(onVector, Vector2.up))
        {
            if (IsCollinear(alongDirection, Vector2.right))
            {
                return new Vector2(0, vector.y);
            }

            var x = 0;
            var y = vector.y - (vector.x * (alongDirection.y / alongDirection.x));

            return new Vector2(x, y);
        }

        if (IsCollinear(alongDirection, Vector2.up))
        {
            if (IsCollinear(onVector, Vector2.right))
            {
                return new Vector2(vector.x, 0);
            }

            var x = (vector.y * onVector.x) / onVector.y;
            var y = vector.y * (onVector.x * onVector.x);

            return new Vector2(x, y);
        }
        else
        {
            var x = ((vector.x * (alongDirection.y / alongDirection.x)) - vector.y) /
                    ((alongDirection.y / alongDirection.x) - (onVector.y / onVector.x));
            var y = x * (onVector.y / onVector.x);

            return new Vector2(x, y);
        }
    }

    /// <summary>
    /// Gets a value indicating whether to vectors are collinear.
    /// </summary>
    /// <param name="a"> Vector "a". </param>
    /// <param name="b"> Vector "b". </param>
    public static bool IsCollinear(Vector3 a, Vector3 b)
    {
        var normsDotProduct = Vector3.Dot(a.normalized, b.normalized);

        return Mathf.Approximately(1, Mathf.Abs(normsDotProduct));
    }

    /// <summary>
    /// Gets a value indicating whether the specified point is space is visible by specified camera.
    /// </summary>
    /// <param name="point"> Point in space. </param>
    /// <param name="camera"> Game camera. </param>
    public static bool IsVisible(this Vector3 point, Camera camera)
    {
        var viewPortPointPosition = camera.WorldToViewportPoint(point);

        return viewPortPointPosition.x >= 0 && viewPortPointPosition.x <= 1 &&
               viewPortPointPosition.y >= 0 && viewPortPointPosition.y <= 1;
    }

    #endregion

    #region [Plane extensions]

    /// <summary>
    /// Creates a plane.
    /// </summary>
    /// <param name="point"> Plane's reference point. </param>
    /// <param name="direction1"> 1st plane's directional vector. </param>
    /// <param name="direction2"> 2nd plane's direcrional vector. </param>
    public static Plane CreatePlane(Vector3 point, Vector3 direction1, Vector3 direction2)
    {
        var translatedPoint1 = point + direction1;
        var translatedPoint2 = point + direction2;

        return new Plane(point, translatedPoint1, translatedPoint2);
    }

    #endregion

    #region [Private methods]

    private static void Copy(Transform source, Dictionary<string, Transform> transforms, string path)
    {
        var name = BuildPath(path, source.name);
        if (transforms.ContainsKey(name))
        {
            return;
        }

        transforms.Add(name, source);
        for (int i = 0; i < source.childCount; i++)
        {
            Copy(source.GetChild(i), transforms, name);
        }
    }

    private static void Paste(Transform destination, Dictionary<string, Transform> transforms, string path)
    {
        var name = BuildPath(path, destination.name);
        if (transforms.ContainsKey(name))
        {
            var currentBone = transforms[name];
            destination.position = currentBone.position;
            destination.rotation = currentBone.rotation;
        }

        for (int i = 0; i < destination.childCount; i++)
        {
            Paste(destination.GetChild(i), transforms, name);
        }
    }

    private static string BuildPath(string path, string name)
    {
        return string.Format("{0}/{1}", path, name);
    }

    private static Transform GetMostRootParentWithSelf(Transform transform)
    {
        var transformParent = transform.parent;

        if (transformParent)
        {
            return transformParent.GetMostRootParent();
        }

        return transform;
    }

    #endregion
}