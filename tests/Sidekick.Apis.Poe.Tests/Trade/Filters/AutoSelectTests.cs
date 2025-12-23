using Sidekick.Apis.Poe.Trade.Trade.Filters.AutoSelect;
using Sidekick.Apis.Poe.Trade.Trade.Filters.Types;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Trade.Filters;

public class AutoSelectTests
{
    [Fact]
    public void HasChangedFromDefaults_ReturnsFalse_WhenSame()
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

        var filter = new TradeFilter
        {
            AutoSelect = preferences,
            DefaultAutoSelect = defaultPreferences
        };

        Assert.False(filter.IsAutoSelectDefaults);
    }

    [Fact]
    public void HasChangedFromDefaults_ReturnsTrue_WhenModeChanged()
    {
        var filter = new TradeFilter
        {
            AutoSelect = new AutoSelectPreferences { Mode = AutoSelectMode.Always },
            DefaultAutoSelect = new AutoSelectPreferences { Mode = AutoSelectMode.Never }
        };

        Assert.True(filter.IsAutoSelectDefaults);
    }

    [Fact]
    public void HasChangedFromDefaults_ReturnsTrue_WhenRuleCheckedChanged()
    {
        var filter = new TradeFilter
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

        Assert.True(filter.IsAutoSelectDefaults);
    }

    [Fact]
    public void HasChangedFromDefaults_ReturnsTrue_WhenConditionValueChanged()
    {
        var filter = new TradeFilter
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

        Assert.True(filter.IsAutoSelectDefaults);
    }

    [Fact]
    public void HasChangedFromDefaults_ReturnsFalse_WhenBothNull()
    {
        var filter = new TradeFilter
        {
            AutoSelect = null,
            DefaultAutoSelect = null
        };

        Assert.False(filter.IsAutoSelectDefaults);
    }

    [Fact]
    public void HasChangedFromDefaults_ReturnsTrue_WhenOneIsNull()
    {
        var filter1 = new TradeFilter
        {
            AutoSelect = new AutoSelectPreferences(),
            DefaultAutoSelect = null
        };

        var filter2 = new TradeFilter
        {
            AutoSelect = null,
            DefaultAutoSelect = new AutoSelectPreferences()
        };

        Assert.True(filter1.IsAutoSelectDefaults);
        Assert.True(filter2.IsAutoSelectDefaults);
    }
}