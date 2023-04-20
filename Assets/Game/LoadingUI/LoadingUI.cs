using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Funzilla
{
	internal class LoadingUI : MonoBehaviour
	{
		[SerializeField] private Image background;
		[SerializeField] private RectTransform progressBar;
		[SerializeField] private RectTransform progressBg;
		[SerializeField] private float fakeLoadTime = 1.0f;

		private void Awake()
		{
			var width = progressBg.sizeDelta.x;
			var size = progressBar.sizeDelta;
			DOVirtual
				.Float(0, 1, fakeLoadTime, t =>
				{
					size.x = width * t;
					progressBar.sizeDelta = size;
				})
				.OnComplete(() =>
				{
					size.x = width;
					progressBar.sizeDelta = size;
					Destroy(gameObject);
				});
		}
	}
}