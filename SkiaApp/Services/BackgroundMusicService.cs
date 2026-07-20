using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Maui.Audio;


namespace SkiaApp.Services
{
    public class BackgroundMusicService
    {

        private readonly IAudioManager _audioManager;

        private IAudioPlayer? _player;


        public BackgroundMusicService(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }



        public async Task PlayAsync()
        {

            if (_player != null)
                return;


            var stream = await FileSystem.OpenAppPackageFileAsync(
                "scizzieost.mp3"
            );


            _player = _audioManager.CreatePlayer(stream);


            _player.Loop = true;

            _player.Volume = 0.35;


            _player.Play();

        }



        public void Stop()
        {

            _player?.Stop();

        }



        public void SetVolume(double volume)
        {

            if (_player != null)
                _player.Volume = volume;

        }

    }
}
