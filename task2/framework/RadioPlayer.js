import { Audio } from 'expo-av';

class RadioPlayer {

    static _createInstance() {
        instance = new RadioPlayer()
        instance._audio = new Audio.Sound();

        return instance;
    }

    async play({ uri }) {
        bResult = true;

        try {

            await this._audio.loadAsync(
                { uri: uri },
            );

            await this._audio.playAsync();

        } catch (e) {
            console.log("[RadioPlayer] Loading '" + uri + "' has failed with " + e);
            bResult = false;
        }

        return bResult;
    }

    async stop() {
        await this._audio.unloadAsync();
    }

    async getStatus() {
        return await this._audio.getStatusAsync();
    }

    static instance = RadioPlayer.instance || RadioPlayer._createInstance();
}

export default RadioPlayer;
