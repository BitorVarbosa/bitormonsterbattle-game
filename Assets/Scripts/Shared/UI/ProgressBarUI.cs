using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace BitorTools
{
    public abstract class ProgressBarUI : MonoBehaviour
    {
        [SerializeField] private Image _fillBar;
        [SerializeField] private TMP_Text _valueText;
        [SerializeField] private float _transitionTime = 0f;
        [SerializeField] private AnimationCurve _transitionInterpolation = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Optional Back Fill (Difference Indicator)")]
        [SerializeField] private Image _backFillBar;
        [SerializeField] private float _backTransitionTime = 1.0f;
        [SerializeField] private AnimationCurve _backTransitionInterpolation = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float _backFillDelay = 0.5f;

        [Header("Text Display Options")]
        [SerializeField] private bool _showPercentage = false;

        private Coroutine _updateFillRoutine;
        private Coroutine _updateBackRoutine;
        private float _currentDisplayValue;
        private float _targetValue;

        protected abstract float GetMaxValue();
        protected abstract float GetCurrentValue();

        protected virtual void Start()
        {
            InitializeBar();
        }

        private void InitializeBar()
        {
            float maxValue = GetMaxValue();
            float currentValue = GetCurrentValue();

            if (maxValue > 0)
            {
                float fillAmount = currentValue / maxValue;
                _fillBar.fillAmount = fillAmount;

                if (_backFillBar != null)
                    _backFillBar.fillAmount = fillAmount;

                _currentDisplayValue = currentValue;
                _targetValue = currentValue;
                UpdateText();
            }
        }

        public void UpdateBar()
        {
            float maxValue = GetMaxValue();
            float currentValue = GetCurrentValue();

            if (maxValue <= 0) return;

            float newFillAmount = currentValue / maxValue;
            _targetValue = currentValue;

            // Handle main fill bar
            if (_transitionTime <= 0)
            {
                // Instant update for main fill
                _fillBar.fillAmount = newFillAmount;
                _currentDisplayValue = currentValue;
                UpdateText();
            }
            else
            {
                // Animated update for main fill
                if (_updateFillRoutine != null)
                    StopCoroutine(_updateFillRoutine);

                _updateFillRoutine = StartCoroutine(UpdateBarRoutine(_fillBar, newFillAmount, _transitionTime, _transitionInterpolation, 0, true));
            }

            // Handle back fill bar independently - only if value decreased and back fill exists
            if (_backFillBar != null && newFillAmount < _fillBar.fillAmount)
            {
                if (_backTransitionTime <= 0)
                {
                    // Instant update for back fill
                    _backFillBar.fillAmount = newFillAmount;
                }
                else
                {
                    // Animated update for back fill
                    if (_updateBackRoutine != null)
                        StopCoroutine(_updateBackRoutine);

                    _updateBackRoutine = StartCoroutine(UpdateBarRoutine(_backFillBar, newFillAmount, _backTransitionTime, _backTransitionInterpolation, _backFillDelay, false));
                }
            }
        }

        private IEnumerator UpdateBarRoutine(Image targetBar, float targetFillAmount, float transitionTime, AnimationCurve interpolationCurve, float delay, bool updateText)
        {
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }

            float startFillAmount = targetBar.fillAmount;
            float startDisplayValue = _currentDisplayValue;
            float elapsedTime = 0f;

            while (elapsedTime < transitionTime)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / transitionTime;

                // Apply animation curve for smooth interpolation
                float curveValue = interpolationCurve.Evaluate(progress);

                // Update fill amount using the evaluated value
                targetBar.fillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, curveValue);

                if (updateText)
                {
                    _currentDisplayValue = Mathf.Lerp(startDisplayValue, _targetValue, curveValue);
                    UpdateText();
                }

                yield return null;
            }

            // Ensure we end exactly at target values
            targetBar.fillAmount = targetFillAmount;

            if (updateText)
            {
                _currentDisplayValue = _targetValue;
                UpdateText();
            }

            // Clear the appropriate coroutine reference
            if (targetBar == _fillBar)
                _updateFillRoutine = null;
            else if (targetBar == _backFillBar)
                _updateBackRoutine = null;
        }

        private void UpdateText()
        {
            if (_valueText == null) return;

            if (_showPercentage)
            {
                float percentage = (GetMaxValue() > 0) ? _currentDisplayValue / GetMaxValue() * 100f : 0f;
                _valueText.text = $"{percentage}%";
            }
            else
            {
                _valueText.text = $"{_currentDisplayValue:0}/{GetMaxValue():0}";
            }
        }

        // Public method to force immediate update without animation
        public void SetBarImmediate()
        {
            float maxValue = GetMaxValue();
            float currentValue = GetCurrentValue();

            if (_updateFillRoutine != null)
            {
                StopCoroutine(_updateFillRoutine);
                _updateFillRoutine = null;
            }

            if (_updateBackRoutine != null)
            {
                StopCoroutine(_updateBackRoutine);
                _updateBackRoutine = null;
            }

            if (maxValue > 0)
            {
                float fillAmount = currentValue / maxValue;
                _fillBar.fillAmount = fillAmount;

                if (_backFillBar != null)
                    _backFillBar.fillAmount = fillAmount;

                _currentDisplayValue = currentValue;
                _targetValue = currentValue;
                UpdateText();
            }
        }

        // Utility method to check if bar is currently animating
        public bool IsAnimating()
        {
            return _updateFillRoutine != null || _updateBackRoutine != null;
        }
    }
}