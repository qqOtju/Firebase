using System.Collections;
using TMPro;
using UnityEngine;

namespace Animations
{
    public class SuccessAnimation : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Color[] _colors;

        private void Start() =>
            StartCoroutine(Animation());

        private IEnumerator Animation()
        {
            if(_colors.Length < 2) yield break;
            for (var i = 0; i < _colors.Length; i++)
            {
                if (i == _colors.Length - 1)
                    yield return StartCoroutine(Lerp(_colors[i], _colors[0]));
                else
                    yield return StartCoroutine(Lerp(_colors[i], _colors[i + 1]));
            }
            StartCoroutine(Animation());
        }

        private IEnumerator Lerp(Color a, Color b)
        {
            var value = 0f;
            while (value < 1)
            {
                _text.color = Color.Lerp(a, b, value);
                value += Time.deltaTime;
                yield return null;
            }            
        }
    }
}