// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Visualisation;
using osu.Framework.Statistics;

namespace osu.Framework.Graphics.Performance
{
    /// <summary>
    /// Tracks game statistics on a global.
    /// </summary>
    internal class GlobalStatisticsDisplay : ToolWindow
    {
        private readonly FillFlowContainer<StatisticsGroup> groups;

        public GlobalStatisticsDisplay()
            : base("Global Statistics", "(Ctrl+F2 to toggle)")
        {
            ScrollContent.Children = new Drawable[]
            {
                groups = new AlphabeticalFlow<StatisticsGroup>
                {
                    Padding = new MarginPadding(5),
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            GlobalStatistics.Statistics.ItemsAdded += add;
            GlobalStatistics.Statistics.ItemsRemoved += remove;
            add(GlobalStatistics.Statistics);
        }

        private void remove(IEnumerable<IGlobalStatistic> stats) => Schedule(() =>
        {
            foreach (var stat in stats)
                groups.FirstOrDefault(g => g.GroupName == stat.Group)?.Remove(stat);
        });

        private void add(IEnumerable<IGlobalStatistic> stats) => Schedule(() =>
        {
            foreach (var stat in stats)
            {
                var group = groups.FirstOrDefault(g => g.GroupName == stat.Group);

                if (group == null)
                    groups.Add(group = new StatisticsGroup(stat.Group));
                group.Add(stat);
            }
        });

        private class StatisticsGroup : CompositeDrawable, IAlphabeticalSort
        {
            public string GroupName { get; }

            private readonly FillFlowContainer<StatisticsItem> items;

            public StatisticsGroup(string groupName)
            {
                GroupName = groupName;

                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;

                InternalChildren = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Children = new Drawable[]
                        {
                            new SpriteText
                            {
                                Text = GroupName,
                                Font = FrameworkFont.Regular.With(weight: "Bold")
                            },
                            items = new AlphabeticalFlow<StatisticsItem>
                            {
                                Padding = new MarginPadding { Left = 5 },
                                RelativeSizeAxes = Axes.X,
                                AutoSizeAxes = Axes.Y,
                                Direction = FillDirection.Vertical,
                            },
                        }
                    },
                };
            }

            public void Add(IGlobalStatistic stat)
            {
                if (items.Any(s => s.Statistic == stat))
                    return;

                items.Add(new StatisticsItem(stat));
            }

            public void Remove(IGlobalStatistic stat)
            {
                items.FirstOrDefault(s => s.Statistic == stat)?.Expire();
            }

            private class StatisticsItem : CompositeDrawable, IAlphabeticalSort
            {
                public readonly IGlobalStatistic Statistic;

                public StatisticsItem(IGlobalStatistic statistic)
                {
                    Statistic = statistic;

                    SpriteText valueText;

                    RelativeSizeAxes = Axes.X;
                    AutoSizeAxes = Axes.Y;

                    InternalChildren = new Drawable[]
                    {
                        new SpriteText
                        {
                            Font = FrameworkFont.Regular,
                            Colour = FrameworkColour.Yellow,
                            Text = Statistic.Name,
                            RelativeSizeAxes = Axes.X,
                            Width = 0.68f,
                        },
                        valueText = new SpriteText
                        {
                            Font = FrameworkFont.Condensed,
                            RelativePositionAxes = Axes.X,
                            X = 0.7f,
                            RelativeSizeAxes = Axes.X,
                            Width = 0.3f,
                        },
                    };

                    Statistic.DisplayValue.BindValueChanged(val => Schedule(() => valueText.Text = val.NewValue), true);
                }

                public string SortString => Statistic.Name;
            }

            public string SortString => GroupName;
        }

        private interface IAlphabeticalSort
        {
            string SortString { get; }
        }

        private class AlphabeticalFlow<T> : FillFlowContainer<T> where T : Drawable, IAlphabeticalSort
        {
            public override IEnumerable<Drawable> FlowingChildren => base.FlowingChildren.Cast<T>().OrderBy(d => d.SortString);
        }
    }
}
