using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PeelMinigameController : MinigameController
{
    // References
    [SerializeField] private Transform _rightPoint;
    [SerializeField] private Transform _middlePoint;
    [SerializeField] private Transform _leftPoint;
    [SerializeField] private Transform[] _macaxeiras;

    // Variables
    [SerializeField] private float _macaxeiraSpeed;
    private int _partsPeeled;
    private int _totalParts = 3;
    private int _totalMacaxeiras = 5;
    private int _macaxeierasPeeled;
    private int _macaxeiraIndex;
    private bool _isMoving;

    protected override void Start()
    {
        EventManager.OnPeelMacaxeiraPointEvent += PeelPart;

        base.Start();
        StartCoroutine(TimeCountdownDelay(_minigameTimerValue));

        _partsPeeled = 0;
        _macaxeierasPeeled = 0;
        _macaxeiraIndex = 0;
        _isMoving = true;

        MoveMacaxeira(_macaxeiraIndex);
    }

    protected override void OnDestroy()
    {
        EventManager.OnPeelMacaxeiraPointEvent -= PeelPart;
        base.OnDestroy();
    }

    private void Update()
    {
        if (_isCountingDown)
            TimerCountdown(_minigameClock, _minigameTimerValue);

        if(_isMoving)
        {
            MoveMacaxeira(_macaxeiraIndex);
        }
    }

    private void PeelPart()
    {
        _partsPeeled += 1;
        if(_partsPeeled >= _totalParts)
        {
            _partsPeeled = 0;
            _macaxeiraIndex += 1;
            if (_macaxeiraIndex < _macaxeiras.Length)
                _isMoving = true;
            else
                EventManager.OnGameWinTrigger();
        }
    }

    private void MoveMacaxeira(int index)
    {
        if(index == 0)
        {
            if (Vector3.Distance(_macaxeiras[index].position, _middlePoint.position) < 0.001f)
            {
                _isMoving = false;
            }
            else
                _macaxeiras[index].position = Vector3.MoveTowards(_macaxeiras[index].position, _middlePoint.position, _macaxeiraSpeed);
        }
        else
        {
            if (Vector3.Distance(_macaxeiras[index].position, _middlePoint.position) < 0.001f)
            {
                _isMoving = false;
            }
            else
            {
                _macaxeiras[index - 1].position = Vector3.MoveTowards(_macaxeiras[index - 1].position, _leftPoint.position, _macaxeiraSpeed);

                _macaxeiras[index].position = Vector3.MoveTowards(_macaxeiras[index].position, _middlePoint.position, _macaxeiraSpeed);
            }
        }
    }
}
