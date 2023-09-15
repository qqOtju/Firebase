using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Loading
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Image _progressFill;
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI _loadingInfo;
        [Header("Values")]
        [SerializeField] private float _barSpeed = 10f;
        [SerializeField] private float _delayBeforeNextLoad = 0f;
        [SerializeField] private AnimationCurve _endAnim;
        
        private readonly WaitForSeconds _wfs = new(0.6f);
        private readonly Color _startColor = Color.red;
        private readonly Color _endColor = Color.green;
        
        private CanvasGroup _canvasGroup;
        private float _targetProgress;
        private bool _isProgress;
        private Canvas _canvas;

        private void Start()
        {
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public async Task Load(Queue<ILoadingOperation> loadingOperations)
        {
            _canvas.enabled = true;
            foreach (var operation in loadingOperations)
            {
                var cor = StartCoroutine(UpdateProgressBar());
                ResetFill();
                _loadingInfo.text = operation.Description;
                operation.OnDescriptionChange += UpdateText; 
                await operation.Load(OnProgress);
                await WaitForBarFill();
                StopCoroutine(cor);
                operation.OnDescriptionChange -= UpdateText;
                await EndProgressBar();
                await DelayBeforeNextLoad();
            }
            await CloseAnimation();
        }

        private void UpdateText(string text) => 
            _loadingInfo.text = text;
        
        private void ResetFill()
        {
            _progressFill.fillAmount = 0;
            _targetProgress = 0;
        }

        private void OnProgress(float progress) => 
            _targetProgress = progress;

        private async Task WaitForBarFill()
        {
            while (_progressFill.fillAmount < _targetProgress)
                await Task.Delay(1);
        }
        
        private async Task DelayBeforeNextLoad() => 
            await Task.Delay(TimeSpan.FromSeconds(_delayBeforeNextLoad));
        
        private IEnumerator UpdateProgressBar()
        {
            while (_canvas.enabled)
            {
                if(_progressFill.fillAmount < _targetProgress)
                {
                    _progressFill.fillAmount += Time.deltaTime * _barSpeed;
                    _progressFill.color = Color.Lerp(_startColor, _endColor, _progressFill.fillAmount);
                }
                yield return null;
            }
        }

        private async Task EndProgressBar()
        {
            var value = 1f;
            while (_progressFill.fillAmount > 0)
            {
                value -= Time.deltaTime;
                _progressFill.fillAmount -= _endAnim.Evaluate(1 - value);
                await Task.Yield();
            }
        }

        private async Task CloseAnimation()
        {
            var value = 1f;
            while (value >= 0f)
            {
                _canvasGroup.alpha = Mathf.Lerp(0, 1, value);
                value -= Time.deltaTime;
                await Task.Delay(1);
            }
            _canvasGroup.alpha = 0;
            _canvas.enabled = false;
        }
    }
}