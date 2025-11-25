using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelSelectController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<SongLevelSO> _allSongs;
    [SerializeField] private VisualTreeAsset _songCardTemplate;

    [Header("UI References")]
    private UIDocument _doc;
    private ScrollView _listContainer;
    
    // Right Panel References
    private VisualElement _detailsContainer; // The container we animate
    private VisualElement _largeCover;
    private Label _largeTitle;
    private Label _highScore;
    private VisualElement _emptyState;
    private bool _hasSelectedFirstSong = false;

    private void OnEnable()
    {
        _doc = GetComponent<UIDocument>();
        VisualElement root = _doc.rootVisualElement;

        _listContainer = root.Q<ScrollView>("SongListContainer");
        
        // Find Right Panel Elements
        _detailsContainer = root.Q<VisualElement>("DetailsContentContainer"); // Check your UXML name!
        _largeCover = root.Q<VisualElement>("LargeCoverArt");
        _largeTitle = root.Q<Label>("LargeTitle");
        _highScore = root.Q<Label>("HighScoreLabel");
        // _detailsContainer = root.Q<VisualElement>("DetailsContentContainer");
        _emptyState = root.Q<VisualElement>("EmptyStateContainer");

        // INITIAL STATE:
        // Show Empty State, Hide Details
        _emptyState.style.display = DisplayStyle.Flex;
        _detailsContainer.style.display = DisplayStyle.None; // Hide the details so they don't pop in

        GenerateSongList();
    }

    private void GenerateSongList()
    {
        _listContainer.Clear();

        foreach (SongLevelSO song in _allSongs)
        {
            TemplateContainer cardInstance = _songCardTemplate.Instantiate();

            // Populate Data
            cardInstance.Q<Label>("TitleLabel").text = $"{song.songTitle}-{song.artistName}";
            // cardInstance.Q<Label>("ArtistLabel").text = song.artistName;
            cardInstance.Q<VisualElement>("CoverArt").style.backgroundImage = new StyleBackground(song.coverArt);
            cardInstance.Q<Label>("RankLabel").text = $"Rank {song.rank}";
            // We pass 'cardInstance' so OnSongSelected knows exactly which card was clicked
            cardInstance.RegisterCallback<ClickEvent>(evt => OnSongSelected(song, cardInstance));

            _listContainer.Add(cardInstance);
        }
    }

    private void OnSongSelected(SongLevelSO song, VisualElement clickedCard)
    {
        // 1. VISUAL HIGHLIGHT (Left Panel)
        // Remove 'selected' class from all cards
        foreach (var child in _listContainer.Children())
        {
            child.RemoveFromClassList("song-card-selected");
        }
        // Add 'selected' class to the clicked card
        clickedCard.AddToClassList("song-card-selected");


        // 2. UPDATE DATA (Right Panel)
        _largeTitle.text = song.songTitle;
        _highScore.text = $"High Score: {song.highScore.ToString("D5")}";;
        _largeCover.style.backgroundImage = new StyleBackground(song.coverArt);


        // 3. TRIGGER ANIMATION (Right Panel)
        // Reset state
        _detailsContainer.RemoveFromClassList("details-content-active");

        // Wait one frame, then activate transition
        _detailsContainer.schedule.Execute(() =>
        {
            _detailsContainer.AddToClassList("details-content-active");
        });

        if (!_hasSelectedFirstSong)
        {
            _hasSelectedFirstSong = true;
            
            // Hide the empty state overlay
            _emptyState.style.display = DisplayStyle.None;
            
            // Show the details container
            _detailsContainer.style.display = DisplayStyle.Flex;
        }
        
        // TODO: Trigger Audio Preview here
    }
}