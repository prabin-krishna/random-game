using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Card : MonoBehaviour
{
    [SerializeField] private Image _cardImage;
    [SerializeField] private Image _backImage;
    [SerializeField] private Button _cardButton;
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioSource _audioSource;
    public event Action<int> OnCardClicked;
    private int _cardId;
    private int _imageId;
    private Sprite _resultSprite;
    private bool _isRevealed = false;
    private bool _isDestroyed = false;
    private bool _isAnimating = false;
    private bool _isInInitSequence = false;
    private void Start()
    {
        if (_cardButton == null)
            _cardButton = GetComponent<Button>();
        _cardButton.onClick.AddListener(HandleCardClick);
        ToggleHideCard(true);
    }
    public void SetCardId(int cardId)
    {
        _cardId = cardId;
    }
    public void SetImageId(int imageId)
    {
        _imageId = imageId;
    }
    public void SetResultImage(Sprite sprite)
    {
        _resultSprite = sprite;
        if (_cardImage != null)
        {
            _cardImage.sprite = sprite;
        }
    }
    public void ToggleHideCard(bool hide)
    {
        if (_isDestroyed) return;
        Debug.Log($"ToggleHideCard called with hide={hide} for Card ID {_cardId}");
        _isRevealed = !hide;
        _isAnimating = true;
        if (_animator != null)
        {
            _animator.ResetTrigger("Flip");
            _animator.ResetTrigger("Reverse");
            if (hide)
            {
                _animator.SetTrigger("Reverse");
                StartCoroutine(ResetAnimationFlag(0.5f));
            }
            else
            {
                _animator.SetTrigger("Flip");
                StartCoroutine(ResetAnimationFlag(0.5f));
            }
        }
        else
        {
            if (_cardImage != null)
                _cardImage.gameObject.SetActive(!hide);
            if (_backImage != null)
                _backImage.gameObject.SetActive(hide);
            _isAnimating = false;
        }
    }
    public void OnReveal()
    {
        if (_isDestroyed) return;
        FlipSound();
        ToggleHideCard(false);
    }
    public void DestroyCard()
    {
        _isDestroyed = true;
        _cardButton.interactable = false;
        PlayHideSound();
        if (_animator != null)
        {
            _animator.SetTrigger("Hide");
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    public void HideCard()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Hide");
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    public void HandleCardClick()
    {
        if (_isDestroyed || _isRevealed || _isAnimating || _isInInitSequence)
        {
            return;
        }
        OnReveal();
        OnCardClicked?.Invoke(_cardId);
    }
    private IEnumerator ResetAnimationFlag(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isAnimating = false;
    }
    private IEnumerator ResetInitSequenceFlag(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isInInitSequence = false;
    }
    public void ResetToIdleState()
    {
        if (_animator == null) return;
        _animator.ResetTrigger("Flip");
        _animator.ResetTrigger("Reverse");
        _animator.ResetTrigger("Hide");
        _isAnimating = false;
        _isInInitSequence = false;
    }
    public void PlayHideSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCardHide();
        }
    }
    private void FlipSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCardFlip();
        }
        else if (_audioSource != null)
        {
            _audioSource.Play();
        }
    }
    public int GetCardId() => _cardId;
    public int GetImageId() => _imageId;
    public bool IsRevealed() => _isRevealed;
    public bool IsDestroyed() => _isDestroyed;
    public bool IsAnimating() => _isAnimating;
    public void SetInInitSequence(bool inSequence)
    {
        _isInInitSequence = inSequence;
    }
    private void OnDestroy()
    {
        if (_cardButton != null)
            _cardButton.onClick.RemoveListener(HandleCardClick);
    }
}