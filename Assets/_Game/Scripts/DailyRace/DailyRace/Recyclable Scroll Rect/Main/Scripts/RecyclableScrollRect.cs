//MIT License
//Copyright (c) 2020 Mohammed Iqubal Hussain
//Website : Polyandcode.com 

using System;
using System.Collections;
using DG.Tweening;
using Features.Experimental.Scripts.Leaderboard;
using UnityEngine;
using UnityEngine.UI;

namespace PolyAndCode.UI
{
    /// <summary>
    /// Entry for the recycling system. Extends Unity's inbuilt ScrollRect.
    /// </summary>
    public class RecyclableScrollRect : ScrollRect
    {
        [HideInInspector]
        public IRecyclableScrollRectDataSource DataSource;

        public bool IsGrid;
        //Prototype cell can either be a prefab or present as a child to the content(will automatically be disabled in runtime)
        public RectTransform PrototypeCell;
        //If true the intiziation happens at Start. Controller must assign the datasource in Awake.
        //Set to false if self init is not required and use public init API.
        public bool SelfInitialize = true;

        public enum DirectionType
        {
            Vertical,
            Horizontal
        }

        public DirectionType Direction;

        //Segments : coloums for vertical and rows for horizontal.
        public int Segments
        {
            set
            {
                _segments = Math.Max(value, 2);
            }
            get
            {
                return _segments;
            }
        }
        [SerializeField]
        private int _segments;

        private RecyclingSystem _recyclingSystem;
        private Vector2 _prevAnchoredPos;
        
        private float sizeOfOneCell;

        protected override void Start()
        {
            //defafult(built-in) in scroll rect can have both directions enabled, Recyclable scroll rect can be scrolled in only one direction.
            //setting default as vertical, Initialize() will set this again. 
            vertical = true;
            horizontal = false;

            if (!Application.isPlaying) return;

            if (SelfInitialize) Initialize();
        }

        /// <summary>
        /// Initialization when selfInitalize is true. Assumes that data source is set in controller's Awake.
        /// </summary>
        private void Initialize()
        {
            //Contruct the recycling system.
            if (Direction == DirectionType.Vertical)
            {
                _recyclingSystem = new VerticalRecyclingSystem(PrototypeCell, viewport, content, DataSource, IsGrid, Segments);
            }
            else if (Direction == DirectionType.Horizontal)
            {
                _recyclingSystem = new HorizontalRecyclingSystem(PrototypeCell, viewport, content, DataSource, IsGrid, Segments);
            }
            vertical = Direction == DirectionType.Vertical;
            horizontal = Direction == DirectionType.Horizontal;

            _prevAnchoredPos = content.anchoredPosition;
            onValueChanged.RemoveListener(OnValueChangedListener);
            //Adding listener after pool creation to avoid any unwanted recycling behaviour.(rare scenerio)
            StartCoroutine(_recyclingSystem.InitCoroutine(() =>
                                                               onValueChanged.AddListener(OnValueChangedListener)
                                                              ));
            
            sizeOfOneCell = content.sizeDelta.y / (11);
        }

        /// <summary>
        /// public API for Initializing when datasource is not set in controller's Awake. Make sure selfInitalize is set to false. 
        /// </summary>
        public void Initialize(IRecyclableScrollRectDataSource dataSource)
        {
            DataSource = dataSource;
            Initialize();
            sizeOfOneCell = content.sizeDelta.y / DataSource.GetItemCount();
        }

        /// <summary>
        /// Added as a listener to the OnValueChanged event of Scroll rect.
        /// Recycling entry point for recyling systems.
        /// </summary>
        /// <param name="direction">scroll direction</param>
        public void OnValueChangedListener(Vector2 normalizedPos)
        {
            Vector2 dir = content.anchoredPosition - _prevAnchoredPos;
            m_ContentStartPosition += _recyclingSystem.OnValueChangedListener(dir);
            _prevAnchoredPos = content.anchoredPosition;
        }

        /// <summary>
        ///Reloads the data. Call this if a new datasource is assigned.
        /// </summary>
        public void ReloadData()
        {
            ReloadData(DataSource);
        }

        /// <summary>
        /// Overloaded ReloadData with dataSource param
        ///Reloads the data. Call this if a new datasource is assigned.
        /// </summary>
        public void ReloadData(IRecyclableScrollRectDataSource dataSource)
        {
            if (_recyclingSystem != null)
            {
                StopMovement();
                onValueChanged.RemoveListener(OnValueChangedListener);
                _recyclingSystem.DataSource = dataSource;
                StartCoroutine(_recyclingSystem.InitCoroutine(() =>
                                                               onValueChanged.AddListener(OnValueChangedListener)
                                                              ));
                _prevAnchoredPos = content.anchoredPosition;
            }
        }
        
        private Coroutine _moveToSelectedIndexCoroutine;
        
        public void MoveToCell(int index)
        {
            if (_recyclingSystem is VerticalRecyclingSystem system1)
            {
                int distance = CalculateDistance(index);
                
                Debug.Log($"Leaderboard Scrolling // Target Index is : {index} / Bottom Cell Index is : {system1.currentItemCount} / Distance is : {distance}");
                
                if (distance != 0)
                {
                    if (_moveToSelectedIndexCoroutine != null)
                    {
                        StopCoroutine(_moveToSelectedIndexCoroutine);
                    }
                    _moveToSelectedIndexCoroutine = StartCoroutine(IE_MoveToSelectedIndex(distance));
                }
            }

        }
        
        private IEnumerator IE_MoveToSelectedIndex(int distance)
        {

            
            int direction = distance > 0 ? 1 : -1;

            yield return null;

            for (int i = 0; i < Mathf.Abs(distance); i++)
            {
                yield return StartCoroutine(IE_ScrollByOneCell(direction));
            }
            
        }
        
        public void MoveToCellWithTime(int SetIndex, int playerIndex)
        {
            if (_recyclingSystem is VerticalRecyclingSystem system1)
            {
                system1.CenterToCellInstant(SetIndex);
                MoveToCell(playerIndex);
            }
        }
        
        public LeaderboardElement GetElementByPoint(int point)
        {
            if (_recyclingSystem is VerticalRecyclingSystem system1)
            {
                return system1.GetCellByPoint(point);
            }

            return null;
        }
        
        public void SnapToCell(int SetIndex)
        {
            if (_recyclingSystem is VerticalRecyclingSystem system1)
            {
                system1.CenterToCellInstant(SetIndex);
            }
        }

        /// <summary>
        /// Scrolls the content up or down by one cell.
        /// </summary>
        /// <param name="direction">The direction to scroll. Use 1 for down/right, -1 for up/left.</param>
        public void ScrollByOneCell(int direction)
        {
            sizeOfOneCell = content.sizeDelta.y / (11);
            // Ensure direction is either 1 or -1
            direction = Mathf.Clamp(direction, -1, 1);

            // Calculate the offset for one cell. Use width in case of horizontal scrolling.
            float cellOffset = Direction == DirectionType.Vertical ? sizeOfOneCell : sizeOfOneCell; // Assuming sizeOfOneCell is set correctly for both directions.

            // Calculate new position
            Vector2 newPosition = content.anchoredPosition;

            if (Direction == DirectionType.Vertical)
            {
                // For vertical, modify y position
                newPosition.y += cellOffset * direction;
            }
            else
            {
                // For horizontal, modify x position
                newPosition.x += cellOffset * direction;
            }

            // Apply the new position
            content.anchoredPosition = newPosition;
            // Optionally, you can add bounds checking here to ensure content doesn't scroll past its intended limits.
        }
        
        public IEnumerator IE_ScrollByOneCell(int direction)
        {
            
            // Ensure direction is either 1 or -1
            direction = Mathf.Clamp(direction, -1, 1);

            // Calculate the offset for one cell. Use width in case of horizontal scrolling.
            float cellOffset = Direction == DirectionType.Vertical ? sizeOfOneCell : sizeOfOneCell; // Assuming sizeOfOneCell is set correctly for both directions.

            // Calculate new position
            Vector2 newPosition = content.anchoredPosition;

            if (Direction == DirectionType.Vertical)
            {
                // For vertical, modify y position
                newPosition.y += cellOffset * direction;
            }
            else
            {
                // For horizontal, modify x position
                newPosition.x += cellOffset * direction;
            }
            
            float scrollAmount = cellOffset * direction;
            int steps = 3;


            for (int i = 0; i < steps; i++)
            {
                content.anchoredPosition += new Vector2(0, scrollAmount / steps);
                yield return null;
            }


        }
        
        public Vector3 GetContentPositionByDistance(int distance)
        {
            sizeOfOneCell = content.sizeDelta.y / (11);
            // Calculate the offset for one cell. Use width in case of horizontal scrolling.
            float cellOffset = Direction == DirectionType.Vertical ? sizeOfOneCell : sizeOfOneCell; // Assuming sizeOfOneCell is set correctly for both directions.

            // Calculate new position
            Vector2 newPosition = content.anchoredPosition;

            if (Direction == DirectionType.Vertical)
            {
                // For vertical, modify y position
                newPosition.y += cellOffset * distance;
            }
            else
            {
                // For horizontal, modify x position
                newPosition.x += cellOffset * distance;
            }

            // Apply the new position
            // content.anchoredPosition = newPosition;
            return newPosition;
            // Optionally, you can add bounds checking here to ensure content doesn't scroll past its intended limits.
        }
        
        private int CalculateDistance(int index)
        {
            if (_recyclingSystem is VerticalRecyclingSystem system1)
            {
                //
                
                int distance = index - system1.currentItemCount;
                distance += 7;
                return distance;
            }

            return 0;
        }

    }
}