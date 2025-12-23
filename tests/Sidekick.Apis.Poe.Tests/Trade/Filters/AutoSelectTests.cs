using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Trade.Filters;

public class AutoSelectTests
{
    private class TestFilter : TradeFilter { }

    [Fact]
    public void Equals_ReturnsTrue_WhenSame()
    {
        var preferences = new AutoSelectPreferences
        {
            Mode = AutoSelectMode.Conditionally,
            Rules = new List<AutoSelectRule>
            {
                new AutoSelectRule
                {
                    Checked = true,
                    Conditions = new List<AutoSelectCondition>
                    {
                        new AutoSelectCondition
                        {
                            Type = AutoSelectConditionType.GreaterThan,
                            Value = 10,
                            Expression = item => item.Properties.ItemLevel
                        }
                    }
                }
            }
        };

        var defaultPreferences = new AutoSelectPreferences
        {
            Mode = AutoSelectMode.Conditionally,
            Rules = new List<AutoSelectRule>
            {
                new AutoSelectRule
                {
                    Checked = true,
                    Conditions = new List<AutoSelectCondition>
                    {
                        new AutoSelectCondition
                        {
                            Type = AutoSelectConditionType.GreaterThan,
                            Value = 10,
                            Expression = item => item.Properties.ItemLevel
                        }
                    }
                }
            }
        };

        var filter = new TestFilter
        {
            AutoSelect = preferences,
            DefaultAutoSelect = defaultPreferences
        };

        Assert.True(Equals(filter.AutoSelect, filter.DefaultAutoSelect));
    }

    [Fact]
    public void Equals_ReturnsFalse_WhenModeChanged()
    {
        var filter = new TestFilter
        {
            AutoSelect = new AutoSelectPreferences { Mode = AutoSelectMode.Always },
            DefaultAutoSelect = new AutoSelectPreferences { Mode = AutoSelectMode.Never }
        };

        Assert.False(Equals(filter.AutoSelect, filter.DefaultAutoSelect));
    }

    [Fact]
    public void Equals_ReturnsFalse_WhenRuleCheckedChanged()
    {
        var filter = new TestFilter
        {
            AutoSelect = new AutoSelectPreferences
            {
                Rules = new List<AutoSelectRule> { new AutoSelectRule { Checked = true } }
            },
            DefaultAutoSelect = new AutoSelectPreferences
            {
                Rules = new List<AutoSelectRule> { new AutoSelectRule { Checked = false } }
            }
        };

        Assert.False(Equals(filter.AutoSelect, filter.DefaultAutoSelect));
    }

    [Fact]
    public void Equals_ReturnsFalse_WhenConditionValueChanged()
    {
        var filter = new TestFilter
        {
            AutoSelect = new AutoSelectPreferences
            {
                Rules = new List<AutoSelectRule>
                {
                    new AutoSelectRule
                    {
                        Conditions = new List<AutoSelectCondition>
                        {
                            new AutoSelectCondition { Value = 10 }
                        }
                    }
                }
            },
            DefaultAutoSelect = new AutoSelectPreferences
            {
                Rules = new List<AutoSelectRule>
                {
                    new AutoSelectRule
                    {
                        Conditions = new List<AutoSelectCondition>
                        {
                            new AutoSelectCondition { Value = 20 }
                        }
                    }
                }
            }
        };

        Assert.False(Equals(filter.AutoSelect, filter.DefaultAutoSelect));
    }

    [Fact]
    public void Equals_ReturnsTrue_WhenBothNull()
    {
        var filter = new TestFilter
        {
            AutoSelect = null,
            DefaultAutoSelect = null
        };

        Assert.True(Equals(filter.AutoSelect, filter.DefaultAutoSelect));
    }

    [Fact]
    public void Equals_ReturnsFalse_WhenOneIsNull()
    {
        var filter1 = new TestFilter
        {
            AutoSelect = new AutoSelectPreferences(),
            DefaultAutoSelect = null
        };

        var filter2 = new TestFilter
        {
            AutoSelect = null,
            DefaultAutoSelect = new AutoSelectPreferences()
        };

        Assert.False(Equals(filter1.AutoSelect, filter1.DefaultAutoSelect));
        Assert.False(Equals(filter2.AutoSelect, filter2.DefaultAutoSelect));
    }
}