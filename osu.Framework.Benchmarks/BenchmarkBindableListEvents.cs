// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using BenchmarkDotNet.Attributes;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.UserInterface;

namespace osu.Framework.Benchmarks
{
    [MemoryDiagnoser]
    public class BenchmarkBindableListEvents
    {
        private readonly BindableList<int> listInt = new BindableList<int>();
        private readonly BindableList<ColourInfo> listColours = new BindableList<ColourInfo>();
        private readonly BindableList<MenuItem> listItems = new BindableList<MenuItem>();

        private object? sink;

        [GlobalSetup]
        public void GlobalSetup()
        {
            listInt.UnbindEvents();
            listInt.CollectionChanged += (_, e) => sink = e;

            listColours.UnbindEvents();
            listColours.CollectionChanged += (_, e) => sink = e;

            listItems.UnbindEvents();
            listItems.CollectionChanged += (_, e) => sink = e;
        }

        [Benchmark]
        public void AddInts()
        {
            for (int i = 0; i < 100; i++)
            {
                listInt.Add(i);
            }

            listInt.Clear();
        }

        [Benchmark]
        public void AddColours()
        {
            for (int i = 0; i < 100; i++)
            {
                listColours.Add(new ColourInfo { TopLeft = new Colour4(i, i, i, 0) });
            }

            listColours.Clear();
        }

        [Benchmark]
        public void AddItems()
        {
            for (int i = 0; i < 100; i++)
            {
                listItems.Add(new MenuItem("Item", () => { }));
            }

            listItems.Clear();
        }
    }
}
