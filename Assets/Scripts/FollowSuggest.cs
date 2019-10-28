using Newtonsoft.Json;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// Ref: The introduction to Reactive Programming you've been missing - https://gist.github.com/staltz/868e7e9bc2a7b8c1f754
public class FollowSuggest : MonoBehaviour
{
    [SerializeField]
    private Text Suggestion1;
    [SerializeField]
    private Text Suggestion2;
    [SerializeField]
    private Text Suggestion3;


    [SerializeField]
    private Button RefreshButton;


    [SerializeField]
    private Button CloseButton1;
    [SerializeField]
    private Button CloseButton2;
    [SerializeField]
    private Button CloseButton3;


    void Start()
    {
        var refreshClickStream = RefreshButton.OnPointerClickAsObservable();


        var close1ClickStream = CloseButton1.OnPointerClickAsObservable();
        var close2ClickStream = CloseButton2.OnPointerClickAsObservable();
        var close3ClickStream = CloseButton3.OnPointerClickAsObservable();
        

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /*
            var requestStream = refreshClickStream
              .map(function() {
                var randomOffset = Math.floor(Math.random() * 500);
                return 'https://api.github.com/users?since=' + randomOffset;
              });

            var responseStream = requestStream
              .flatMap(function(requestUrl) {
                return Rx.Observable.fromPromise(jQuery.getJSON(requestUrl));
              });

            responseStream.subscribe(function(response) {
              // render `response` to the DOM however you wish
            });
        */

        var requestStream = refreshClickStream.Select((x) =>
        {
            var randomOffset = Random.Range(1, 500);

            return "https://api.github.com/users?since=" + randomOffset;
        }).StartWith("https://api.github.com/users");

        // The StartWith() function does exactly what you think it does. No matter how your input stream looks like,
        // the output stream resulting of StartWith(x) will have x at the beginning.

        var responseStream = requestStream
                                    .Select(url => ObservableWWW.Get(url))
                                    .SelectMany(x => x);

        // For debug

        // responseStream.Subscribe((responseBody) => { Debug.Log(responseBody); });
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        var usersStream = responseStream.Select((responseBody) =>
        {
            return JsonConvert.DeserializeObject<List<User>>(responseBody);
        });


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /*
            var suggestion1Stream = close1ClickStream.startWith('startup click')
                .combineLatest(responseStream,             
                    function(click, listUsers) {
                        // get one random user from the list
                        return listUsers[Math.floor(Math.random()*listUsers.length)];
                    }
                )
                .merge(refreshClickStream.map(function(){ return null; })
                )
                .startWith(null);

            suggestion1Stream.subscribe(function(suggestion) {
                if (suggestion === null) {
                // hide the first suggestion DOM element
                }
                else {
                // show the first suggestion DOM element
                // and render the data
                }
            });
        */

        var suggestion1Stream = close1ClickStream.StartWith(new PointerEventData(EventSystem.current))
            .CombineLatest(usersStream, (click, users) =>
            {
                return users[Random.Range(0, users.Count)].login;

            })
            .Merge(refreshClickStream.Select(x => string.Empty))
            .StartWith(string.Empty);

        suggestion1Stream.Subscribe((suggestion) =>
        {
            if (suggestion == string.Empty)
            {
                // Use placeholder text instead of string.Empty

                // Suggestion1.text = string.Empty;
                Suggestion1.text = "<Empty>";
            }
            else
            {
                Suggestion1.text = suggestion;
            }
        });

        var suggestion2Stream = close2ClickStream.StartWith(new PointerEventData(EventSystem.current))
            .CombineLatest(usersStream, (click, users) =>
            {
                return users[Random.Range(0, users.Count)].login;

            })
            .Merge(refreshClickStream.Select(x => string.Empty))
            .StartWith(string.Empty);

        suggestion2Stream.Subscribe((suggestion) =>
        {
            if (suggestion == string.Empty)
            {
                // Use placeholder text instead of string.Empty

                // Suggestion2.text = string.Empty;
                Suggestion2.text = "<Empty>";
            }
            else
            {
                Suggestion2.text = suggestion;
            }
        });

        var suggestion3Stream = close3ClickStream.StartWith(new PointerEventData(EventSystem.current))
            .CombineLatest(usersStream, (click, users) =>
            {
                return users[Random.Range(0, users.Count)].login;

            })
            .Merge(refreshClickStream.Select(x => string.Empty))
            .StartWith(string.Empty);

        suggestion3Stream.Subscribe((suggestion) =>
        {
            if (suggestion == string.Empty)
            {
                // Use placeholder text instead of string.Empty

                // Suggestion3.text = string.Empty;
                Suggestion3.text = "<Empty>";
            }
            else
            {
                Suggestion3.text = suggestion;
            }
        });
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}