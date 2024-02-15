using System;
using Godot;
using Godot.Collections;

namespace UnstableConcoction.GdBinds;

[GlobalClass]
public partial class PhantomCamera2DWrapper: Node2D
{
    [Export]
    public Node2D PhantomCamera 
    {
        get => _phantomCamera ?? throw new NullReferenceException("PhantomCamera not set");
        set
        {
            _phantomCamera = value;
            SetCameraScript();
        }
    }
    
    private Node2D? _phantomCamera;
    
    private GDScript? PCam { get; set; }
    
    
    private void SetCameraScript()
    {
        PCam = (GDScript) PhantomCamera.GetScript() ?? throw new ArgumentException("Cant find script on PhantomCamera");
        PhantomCamera.Connect("became_active", Callable.From(OnPhantomCameraBecameActive));
        PhantomCamera.Connect("became_inactive", Callable.From(OnPhantomCameraBecameInactive));
        PhantomCamera.Connect("tween_started", Callable.From(OnPhantomCameraTweenStarted));
        PhantomCamera.Connect("is_tweening", Callable.From(OnPhantomCameraIsTweening));
        PhantomCamera.Connect("tween_completed", Callable.From(OnPhantomCameraTweenCompleted));

    }

    #region Priority
    
    public int Priority
    {
        get => (int) PCam!.Call("get_priority");
        set => PCam!.Call("set_priority", value);
    }
    #endregion

    #region FollowGroup
    public Array<Node2D> FollowGroup => (Array<Node2D>) PCam!.Call("get_follow_group_nodes");

    public bool FollowGroupDamping
    {
        get => (bool) PCam!.Call("get_follow_has_damping");
        set => PCam!.Call("set_follow_has_damping", value);
    }

    public float Damping
    {
        get => (float) PCam!.Call("get_follow_damping_value");
        set => PCam!.Call("set_follow_damping_value", value);
    }
    
    public bool AutoZoom
    {
        get => (bool) PCam!.Call("get_auto_zoom");
        set => PCam!.Call("set_auto_zoom", value);
    }
    
    public float MinAutoZoom
    {
        get => (float) PCam!.Call("get_min_auto_zoom");
        set => PCam!.Call("set_min_auto_zoom", value);
    }
    
    public float MaxAutoZoom
    {
        get => (float) PCam!.Call("get_max_auto_zoom");
        set => PCam!.Call("set_max_auto_zoom", value);
    }

    public Vector4 AutoZoomMargin
    {
        get => (Vector4) PCam!.Call("get_zoom_auto_margin");
        set => PCam!.Call("set_zoom_auto_margin", value);
    }
    
    public void AppendToFollowGroup(Node2D newTarget)
    {
        PCam!.Call("append_follow_group_node", newTarget);
    }
    
    public void AppendToFollowGroup(Array<Node2D> newTargets)
    {
        PCam!.Call("append_follow_group_node_array", newTargets);
    }
    
    #endregion

    #region SecondaryProperties

    public bool IsActive => (bool) PCam!.Call("is_active");
    
    public bool TweenOnLoad
    {
        get => (bool) PCam!.Call("is_tween_on_load");
        set => PCam!.Call("set_tween_on_load", value);
    }

    public Vector2 Zoom
    {
        get => (Vector2) PCam!.Call("get_zoom");
        set => PCam!.Call("set_zoom", value);
    }
    #endregion

    #region Signals

    public Action? BecameActive { get; set; }
    
    public void OnPhantomCameraBecameActive()
    {
        BecameActive?.Invoke();
    }
    
    public Action? BecameInactive { get; set; }
    
    public void OnPhantomCameraBecameInactive()
    {
        BecameInactive?.Invoke();
    }
    
    public Action? TweenStarted { get; set; }
    
    public void OnPhantomCameraTweenStarted()
    {
        TweenStarted?.Invoke();
    }
    
    public Action? IsTweening { get; set; }
    
    public void OnPhantomCameraIsTweening()
    {
        IsTweening?.Invoke();
    }
    
    public Action? TweenCompleted { get; set; }
    
    public void OnPhantomCameraTweenCompleted()
    {
        TweenCompleted?.Invoke();
    }
    #endregion
}