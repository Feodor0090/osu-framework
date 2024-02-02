// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System.Collections.Specialized;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Mixing;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace osu.Framework.Graphics.Visualisation.Audio
{
    internal partial class AudioMixerVisualiser : ToolWindow
    {
        [Resolved]
        private AudioManager audioManager { get; set; }

        private readonly FillFlowContainer<MixerDisplay> mixerFlow;
        private readonly IBindableList<AudioMixer> activeMixers = new BindableList<AudioMixer>();

        public AudioMixerVisualiser()
            : base("AudioMixer", "(Ctrl+F9 to toggle)")
        {
            MainHorizontalContent.Clear();
            MainHorizontalContent.Add(new BasicScrollContainer(Direction.Horizontal)
            {
                RelativeSizeAxes = Axes.Y,
                Width = WIDTH,
                Children = new[]
                {
                    mixerFlow = new FillFlowContainer<MixerDisplay>
                    {
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Spacing = new Vector2(10),
                        Padding = new MarginPadding(10)
                    }
                }
            });
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            activeMixers.BindTo(audioManager.ActiveMixers);
            activeMixers.BindCollectionChanged(onActiveMixerHandlesChanged, true);
        }

        private void onActiveMixerHandlesChanged(IBindableList<AudioMixer> sender, CollectionChangedEvent<AudioMixer> e) => Schedule(() =>
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var mixer in e.NewItems)
                        mixerFlow.Add(new MixerDisplay(mixer));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    mixerFlow.RemoveAll(m => e.OldItems.Contains(m.Mixer), true);
                    break;
            }
        });
    }
}
