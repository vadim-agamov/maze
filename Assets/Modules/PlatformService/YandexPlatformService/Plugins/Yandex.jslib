mergeInto(LibraryManager.library, {

    YandexGetUserId: function () {
        var returnStr = FBInstant.player.getID() || "";
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },
    
    // initialize
    YandexStartGame: function () {
        console.log("YandexStartGame");
        ysdk.features.LoadingAPI.ready();

        ysdk.getPlayer().then(_player => {
            player = _player;
            unityInstance.SendMessage('Yandex', 'YandexOnGameStarted');
        }).catch(err => {
            unityInstance.SendMessage('Yandex', 'YandexOnGameNotStarted', error.message);
        });
    },

    // interstitial
    YandexShowInterstitial: function () {
        ysdk.adv.showFullscreenAdv({
            callbacks: {
                onClose: function(wasShown) {
                    console.log("YandexShowInterstitial shown: " + wasShown);
                    unityInstance.SendMessage('Yandex', 'YandexOnInterstitialShown');
                },
                onError: function(error) {
                    console.log("YandexShowInterstitial not shown: " + error);
                    unityInstance.SendMessage('Yandex', 'YandexOnInterstitialNotShown', error);
                }
            }
        })
    },

    // rewarded video
    YandexShowRewardedVideo: function () {

        ysdk.adv.showRewardedVideo({
            callbacks: {
                onOpen: () => {
                    console.log('Video ad open.');
                },
                onRewarded: () => {
                    console.log('Rewarded!');
                },
                onClose: () => {
                    console.log('Video ad closed.');
                    unityInstance.SendMessage('Yandex', 'YandexOnRewardedVideoShown');
                },
                onError: (e) => {
                    console.log('Error while open video ad:', e);
                    unityInstance.SendMessage('Yandex', 'YandexOnRewardedVideoNotShown', e);
                }
            }
        });
    },

    // Profile
    YandexSetData: function (dataStr) {
        var dataRaw = UTF8ToString(dataStr);
        var dataObject = JSON.parse(dataRaw);
        var result = {data: dataObject};
        console.log('SetData begin: ' + result);
        player
            .setData(result)
            .then(function () {
                console.log('SetData done');
            });
    },

    YandexGetData: function () {
        player
            .getData(["data"])
            .then(function (response) {
                console.log('GetData: respose: ' + response);
                var data = JSON.stringify(response["data"] || {});
                console.log('GetData: loaded: ' + data);
                unityInstance.SendMessage('Yandex', 'YandexOnPlayerProgressLoaded', data);
            })
            .catch(function (error) {
                console.log("GetData not loaded: " + error.message);
                unityInstance.SendMessage('Yandex', 'YandexOnPlayerProgressLoaded', "{}");
            });
    },

    // Event
    YandexLogEvent: function (eventName) {
    }
});