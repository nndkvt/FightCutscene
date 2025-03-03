using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Lean.Transition;
using CW.Common;
using System.Collections.Generic;

namespace Lean.Gui
{
    public interface ILeanTooltip
    {
        void SetTooltipTarget(GameObject target);
        void HideTooltip();
    }
    
    [RequireComponent(typeof(RectTransform))]
    [HelpURL(LeanGui.HelpUrlPrefix + "LeanTooltip")]
    [AddComponentMenu(LeanGui.ComponentMenuPrefix + "Tooltip")]
    public class LeanTooltip : MonoBehaviour ,ILeanTooltip
    {
        [System.Serializable] public class UnityEventString : UnityEvent<GameObject>
        {
        }

        public GameObject HoverObject;
        public bool HoverShow;

        public PointerEventData PressPointer;
        public GameObject PressObject;
        public bool PressShow;

        public string _blockTooltipTag = "BlockTooltip";

        public void SetTooltipTarget(GameObject target)
        {
            HoverObject = target;
            HoverShow = true;
        }
        
        public void HideTooltip()
        {
            HoverObject = null;
            HoverShow = false;
        }

        public enum BoundaryType
        {
            None,
            FlipPivot,
            ShiftPosition
        }

        public enum ActivationType
        {
            HoverOrPress,
            Hover,
            Press
        }

        /// <summary>This allows you to control when the tooltip will appear.
        /// HoverOrPress = When the mouse is hovering, or when the mouse/finger is pressing.
        /// Hover = Only when the mouse is hovering.
        /// Press = Only when the mouse/finger is pressing.</summary>
        public ActivationType Activation { set { activation = value; } get { return activation; } }

        [SerializeField] private ActivationType activation;

        /// <summary>This allows you to delay how quickly the tooltip will appear or switch.</summary>
        public float ShowDelay { set { showDelay = value; } get { return showDelay; } }

        [SerializeField] private float showDelay;

        /// <summary>Move the attached Transform when the tooltip is open?</summary>
        public bool Move { set { move = value; } get { return move; } }

        [SerializeField] private bool move = true;

        /// <summary>This allows you to control how the tooltip will behave when it goes outside the screen bounds.
        /// FlipPivot = If the tooltip goes outside one of the screen boundaries, flip its pivot point on that axis so it goes the other way.
        /// ShiftPosition = If the tooltip goes outside of the screen boundaries, shift its position until it's back inside.
        /// NOTE: If <b>FlipPivot</b> is used and the tooltip is larger than the screen size, then it will revert to <b>ShiftPosition</b>.</summary>
        public BoundaryType Boundary { set { boundary = value; } get { return boundary; } }

        [SerializeField] private BoundaryType boundary;

        /// <summary>This allows you to perform a transition when this tooltip appears.
        /// You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
        /// For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.
        /// NOTE: Any transitions you perform here should be reverted in the <b>Hide Transitions</b> setting using a matching transition component.</summary>
        public LeanPlayer ShowTransitions
        {
            get
            {
                if (showTransitions == null)
                    showTransitions = new LeanPlayer();
                return showTransitions;
            }
        }

        [SerializeField] private LeanPlayer showTransitions;

        /// <summary>This allows you to perform a transition when this tooltip hides.
        /// You can create a new transition GameObject by right clicking the transition name, and selecting <b>Create</b>.
        /// For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.</summary>
        public LeanPlayer HideTransitions
        {
            get
            {
                if (hideTransitions == null)
                    hideTransitions = new LeanPlayer();
                return hideTransitions;
            }
        }

        [SerializeField] private LeanPlayer hideTransitions;

        /// <summary>This allows you to perform an action when this tooltip appears.</summary>
        public UnityEventString OnShow
        {
            get
            {
                if (onShow == null)
                    onShow = new UnityEventString();
                return onShow;
            }
        }

        [SerializeField] private UnityEventString onShow;

        /// <summary>This allows you to perform an action when this tooltip hides.</summary>
        public UnityEvent OnHide
        {
            get
            {
                if (onHide == null)
                    onHide = new UnityEvent();
                return onHide;
            }
        }

        [SerializeField] private UnityEvent onHide;

        [System.NonSerialized]
        private GameObject tooltip;

        [System.NonSerialized]
        private RectTransform cachedRectTransform;

        [System.NonSerialized]
        private bool cachedRectTransformSet;

        [System.NonSerialized]
        private float currentDelay;

        [System.NonSerialized]
        private bool shown;

        private static Vector3[] corners = new Vector3[4];

        protected virtual void Update()
        {
            if (cachedRectTransformSet == false)
            {
                InitializeCachedRectTransform();
            }

            GameObject finalData = null;
            Vector2 finalPoint = Vector2.zero;
            cachedRectTransform.pivot = new Vector2(0, 1);
            GetFinalDataAndPoint(ref finalData, ref finalPoint);

            if (tooltip != finalData)
            {
                ResetTooltip(finalData);
            }

            UpdateTooltip(finalData, finalPoint);

            if (move == true && boundary != BoundaryType.None)
            {
                AdjustRectTransformBoundary(finalPoint);
            }
        }

        private void InitializeCachedRectTransform()
        {
            cachedRectTransform = GetComponent<RectTransform>();
            cachedRectTransformSet = true;
        }

        private void GetFinalDataAndPoint(ref GameObject finalData, ref Vector2 finalPoint)
        {
            switch (activation)
            {
                case ActivationType.HoverOrPress:
                {
                    if (HoverShow)
                    {
                        finalData = HoverObject;
                        finalPoint = Input.mousePosition;
                    }
                }
                    break;

                case ActivationType.Hover:
                {
                    if (HoverShow && !PressShow)
                    {
                        finalData = HoverObject;
                        finalPoint = Input.mousePosition;
                    }
                }
                    break;

                case ActivationType.Press:
                {
                    if (PressShow && HoverShow && HoverObject == PressObject)
                    {
                        finalData = PressObject;
                        finalPoint = PressPointer.position;
                    }
                }
                    break;
            }
        }

        private void ResetTooltip(GameObject newTooltip)
        {
            currentDelay = 0.0f;
            tooltip = newTooltip;
            shown = false;

            Hide();
        }

        private void UpdateTooltip(GameObject currentTooltip, Vector3 finalPoint)
        {
            if (currentTooltip != null)
            {
                currentDelay += Time.unscaledDeltaTime;

                if (currentDelay >= showDelay)
                {
                    if (!shown && !AreThereAnyBlockers())
                    {
                        Show();
                    }

                    if (move)
                    {
                        cachedRectTransform.position = finalPoint;
                    }
                }
            }
        }

        private void AdjustRectTransformBoundary(Vector3 finalPoint)
        {
            cachedRectTransform.GetWorldCorners(corners);

            Vector2 min = Vector2.Min(corners[0], Vector2.Min(corners[1], Vector2.Min(corners[2], corners[3])));
            Vector2 max = Vector2.Max(corners[0], Vector2.Max(corners[1], Vector2.Max(corners[2], corners[3])));
            Vector2 pivot = cachedRectTransform.pivot;
            Vector2 position = cachedRectTransform.position;
            BoundaryType boundaryX = boundary;
            BoundaryType boundaryY = boundary;

            if (boundary == BoundaryType.FlipPivot)
            {
                HandleFlipPivotBoundary(ref boundaryX, ref boundaryY, min, max, finalPoint);
            }

            HandleBoundaryX(ref pivot, ref position, boundaryX, min, max);
            HandleBoundaryY(ref pivot, ref position, boundaryY, min, max);

            cachedRectTransform.pivot = pivot;
            cachedRectTransform.position = position;
        }

        private void HandleFlipPivotBoundary(ref BoundaryType boundaryX, ref BoundaryType boundaryY, Vector2 min, Vector2 max, Vector3 finalPoint)
        {
            Vector2 size = max - min;

            if (finalPoint.x - size.x < 0 && finalPoint.x + size.x > Screen.width)
            {
                boundaryX = BoundaryType.ShiftPosition;
            }

            if (finalPoint.y - size.y < 0 && finalPoint.y + size.y > Screen.height)
            {
                boundaryY = BoundaryType.ShiftPosition;
            }
        }

        private void HandleBoundaryX(ref Vector2 pivot, ref Vector2 position, BoundaryType boundaryX, Vector2 min, Vector2 max)
        {
            if (boundaryX == BoundaryType.FlipPivot)
            {
                if (min.x < 0.0f)
                    pivot.x = 0.0f;
                else if (max.x > Screen.width)
                    pivot.x = 1.0f;
            }
            else if (boundaryX == BoundaryType.ShiftPosition)
            {
                if (min.x < 0.0f)
                    position.x -= min.x;
                else if (max.x > Screen.width)
                    position.x -= max.x - Screen.width;
            }
        }

        private void HandleBoundaryY(ref Vector2 pivot, ref Vector2 position, BoundaryType boundaryY, Vector2 min, Vector2 max)
        {
            if (boundaryY == BoundaryType.FlipPivot)
            {
                if (min.y < 0.0f)
                    pivot.y = 0.0f;
                else if (max.y > Screen.height)
                    pivot.y = 1.0f;
            }
            else if (boundaryY == BoundaryType.ShiftPosition)
            {
                if (min.y < 0.0f)
                    position.y -= min.y;
                else if (max.y > Screen.height)
                    position.y -= max.y - Screen.height;
            }
        }

        private void Show()
        {
            shown = true;

            if (showTransitions != null)
            {
                showTransitions.Begin();
            }

            if (onShow != null)
            {
                onShow.Invoke(tooltip);
            }
        }

        private void Hide()
        {
            if (hideTransitions != null)
            {
                hideTransitions.Begin();
            }

            if (onHide != null)
            {
                onHide.Invoke();
            }
        }

        /// <summary>
        /// Блок тултипов
        /// </summary>
        private List<RaycastResult> RaycastMouse()
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };

            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            return results;
        }

        private bool AreThereAnyBlockers()
        {
            var raycasts = RaycastMouse();

            foreach (RaycastResult raycast in raycasts)
            {
                if (raycast.gameObject.tag == _blockTooltipTag)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

#if UNITY_EDITOR
namespace Lean.Gui.Editor
{
    using UnityEditor;
    using TARGET = LeanTooltip;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TARGET))]
    public class LeanTooltip_Editor : CwEditor
    {
        protected override void OnInspector()
        {
            TARGET tgt;
            TARGET[] tgts;
            GetTargets(out tgt, out tgts);

            Draw("activation", "This allows you to control when the tooltip will appear.\nHoverOrPress = When the mouse is hovering, or when the mouse/finger is pressing.\nHover = Only when the mouse is hovering.\nPress = Only when the mouse/finger is pressing.");
            Draw("showDelay", "This allows you to delay how quickly the tooltip will appear or switch.");

            Separator();

            Draw("move", "Move the attached Transform when the tooltip is open?");
            if (Any(tgts, t => t.Move == true))
            {
                BeginIndent();
                Draw("boundary", "This allows you to control how the tooltip will behave when it goes outside the screen bounds.\n\nFlipPivot = If the tooltip goes outside one of the screen boundaries, flip its pivot point on that axis so it goes the other way.\n\nShiftPosition = If the tooltip goes outside of the screen boundaries, shift its position until it's back inside.\n\nNOTE: If <b>FlipPivot</b> is used and the tooltip is larger than the screen size, then it will revert to <b>ShiftPosition</b>.");
                EndIndent();
            }

            Draw("HoverObject");
            Draw("HoverShow");
            Separator();

            Draw("showTransitions", "This allows you to perform a transition when this tooltip appears. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.\n\nNOTE: Any transitions you perform here should be reverted in the Hide Transitions setting using a matching transition component.");
            Draw("hideTransitions", "This allows you to perform a transition when this tooltip hides. You can create a new transition GameObject by right clicking the transition name, and selecting Create. For example, the <b>Graphic.color Transition (LeanGraphicColor)</b> component can be used to change the color.");

            Separator();

            Draw("onShow");
            Draw("onHide");
        }
    }
}
#endif