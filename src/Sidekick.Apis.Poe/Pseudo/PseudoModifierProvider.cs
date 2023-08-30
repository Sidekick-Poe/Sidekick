using System.Text.RegularExpressions;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Apis.Poe.Pseudo.Models;
using Sidekick.Common.Game.Items.Modifiers;
using Sidekick.Common.Initialization;

namespace Sidekick.Apis.Poe.Pseudo
{
    public class PseudoModifierProvider : IPseudoModifierProvider
    {
        private readonly Regex ParseHashPattern = new("\\#");

        private readonly IEnglishModifierProvider englishModifierProvider;

        public PseudoModifierProvider(
            IEnglishModifierProvider englishModifierProvider)
        {
            this.englishModifierProvider = englishModifierProvider;
        }

        private List<PseudoDefinition> Definitions { get; } = new();

        /// <inheritdoc/>
        public InitializationPriority Priority => InitializationPriority.Low;

        /// <inheritdoc/>
        public async Task Initialize()
        {
            if (Definitions.Count > 0)
            {
                return;
            }

            var result = await englishModifierProvider.GetList();
            var groups = InitGroups(result);

            foreach (var category in result)
            {
                var first = category.Entries.FirstOrDefault();
                if (first == null || first.Id?.Split('.').First() == "pseudo")
                {
                    continue;
                }

                foreach (var entry in category.Entries)
                {
                    foreach (var group in groups)
                    {
                        if (group.Exception != null && group.Exception.IsMatch(entry.Text ?? string.Empty))
                        {
                            continue;
                        }

                        foreach (var pattern in group.Patterns)
                        {
                            if (entry.Id == null || entry.Type == null || entry.Text == null)
                            {
                                continue;
                            }

                            if (pattern.Pattern.IsMatch(entry.Text))
                            {
                                pattern.Matches.Add(new PseudoPatternMatch(entry.Id, entry.Type, entry.Text));
                            }
                        }
                    }
                }
            }

            foreach (var group in groups)
            {
                if (group.Text == null)
                {
                    continue;
                }

                var definition = new PseudoDefinition(group.Id, group.Text);

                foreach (var pattern in group.Patterns)
                {
                    PseudoDefinitionModifier? modifier = null;

                    foreach (var match in pattern.Matches.OrderBy(x => x.Type).ThenBy(x => x.Text.Length))
                    {
                        if (modifier != null)
                        {
                            if (modifier.Type != match.Type)
                            {
                                modifier = null;
                            }
                            else if (!match.Text.StartsWith(modifier.Text))
                            {
                                modifier = null;
                            }
                        }

                        if (modifier == null)
                        {
                            modifier = new PseudoDefinitionModifier(match.Type, match.Text, pattern.Multiplier);
                        }

                        modifier.Ids.Add(match.Id);

                        if (!definition.Modifiers.Contains(modifier))
                        {
                            definition.Modifiers.Add(modifier);
                        }
                    }
                }

                Definitions.Add(definition);
            }
        }

        private static List<PseudoPatternGroup> InitGroups(List<ApiCategory> categories)
        {
            var pseudoCategory = categories
                .FirstOrDefault(x => x.Entries.FirstOrDefault()?.Id?.Split('.').FirstOrDefault() == "pseudo");
            if (pseudoCategory == null)
            {
                return new();
            }

            var groups = new List<PseudoPatternGroup>() {
                // +#% total to Cold Resistance
                new PseudoPatternGroup(
                    apiCategory: pseudoCategory,
                    id: "pseudo.pseudo_total_cold_resistance",
                    exception: new Regex("Minions|Enemies|Totems"),
                    new PseudoPattern(new Regex("to Cold Resistance$")),
                    new PseudoPattern(new Regex("(?=.*Cold)to (?:Fire|Cold|Lightning|Chaos) and (?:Fire|Cold|Lightning|Chaos) Resistances$"))),

                // +#% total to Fire Resistance
                new PseudoPatternGroup(
                    apiCategory: pseudoCategory,
                    id: "pseudo.pseudo_total_fire_resistance",
                    exception: new Regex("Minions|Enemies|Totems"),
                    new PseudoPattern(new Regex("to Fire Resistance$")),
                    new PseudoPattern(new Regex("(?=.*Fire)to (?:Fire|Cold|Lightning|Chaos) and (?:Fire|Cold|Lightning|Chaos) Resistances$"))),

                // +#% total to Lightning Resistance
                new PseudoPatternGroup(
                    apiCategory: pseudoCategory,
                    id: "pseudo.pseudo_total_lightning_resistance",
                    exception: new Regex("Minions|Enemies|Totems"),
                    new PseudoPattern(new Regex("to Lightning Resistance$")),
                    new PseudoPattern(new Regex("(?=.*Lightning)to (?:Fire|Cold|Lightning|Chaos) and (?:Fire|Cold|Lightning|Chaos) Resistances$"))),

                // +#% total to Chaos Resistance
                new PseudoPatternGroup(
                    apiCategory: pseudoCategory,
                    id: "pseudo.pseudo_total_chaos_resistance",
                    exception: new Regex("Minions|Enemies|Totems"),
                    new PseudoPattern(new Regex("to Chaos Resistance$")),
                    new PseudoPattern(new Regex("(?=.*Chaos)to (?:Fire|Cold|Lightning|Chaos) and (?:Fire|Cold|Lightning|Chaos) Resistances$"))),

                // +#% total to all Elemental Resistances
                new PseudoPatternGroup(
                    apiCategory: pseudoCategory,
                    id: "pseudo.pseudo_total_all_elemental_resistances",
                    exception: new Regex("Minions|Enemies|Totems"),
                    new PseudoPattern(new Regex("to all Elemental Resistances$"))),

                // +#% total Elemental Resistance
                new PseudoPatternGroup(
                    apiCategory: pseudoCategory,
                    id: "pseudo.pseudo_total_elemental_resistance",
                    exception: new Regex("Minions|Enemies|Totems"),
                    new PseudoPattern(new Regex("to (?:Fire|Cold|Lightning) Resistance$")),
                    new PseudoPattern(new Regex("to (?:Fire|Cold|Lightning) and (?:Fire|Cold|Lightning) Resistances$"), 2),
                    new PseudoPattern(new Regex("(?=.*Chaos)to (?:Fire|Cold|Lightning|Chaos) and (?:Fire|Cold|Lightning|Chaos) Resistances$")),
                    new PseudoPattern(new Regex("to all Elemental Resistances$"), 3)),

                // +#% total Resistance
                new PseudoPatternGroup(
                    apiCategory: pseudoCategory,
                    id: "pseudo.pseudo_total_resistance",
                    exception: new Regex("Minions|Enemies|Totems"),
                    new PseudoPattern(new Regex("to (Fire|Cold|Lightning|Chaos) Resistance$")),
                    new PseudoPattern(new Regex("to (?:Fire|Cold|Lightning|Chaos) and (?:Fire|Cold|Lightning|Chaos) Resistances$"), 2),
                    new PseudoPattern(new Regex("to all Elemental Resistances$"), 3)),

                // # total Resistances
                // pseudo.pseudo_count_resistances

                // +# total to Strength
                new PseudoPatternGroup(
                    apiCategory: pseudoCategory,
                    id: "pseudo.pseudo_total_strength",
                    exception: new Regex("Passive"),
                    new PseudoPattern(new Regex("to Strength$")),
                    new PseudoPattern(new Regex("(?=.*Strength)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$")),
                    new PseudoPattern(new Regex("to all Attributes$"))),

                // +# total to Dexterity
                new PseudoPatternGroup(
                    apiCategory: pseudoCategory,
                    id: "pseudo.pseudo_total_dexterity",
                    exception: new Regex("Passive"),
                    new PseudoPattern(new Regex("to Dexterity$")),
                    new PseudoPattern(new Regex("(?=.*Dexterity)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$")),
                    new PseudoPattern(new Regex("to all Attributes$"))),

                // +# total to Intelligence
                new PseudoPatternGroup(
                    apiCategory: pseudoCategory,
                    id: "pseudo.pseudo_total_intelligence",
                    exception: new Regex("Passive"),
                    new PseudoPattern(new Regex("to Intelligence$")),
                    new PseudoPattern(new Regex("(?=.*Intelligence)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$")),
                    new PseudoPattern(new Regex("to all Attributes$"))),

                // +# total to all Attributes
                new PseudoPatternGroup(
                    apiCategory: pseudoCategory,
                    id: "pseudo.pseudo_total_all_attributes",
                    exception: new Regex("Passive"),
                    new PseudoPattern(new Regex("to all Attributes$"))),

                // +# total maximum Life
                new PseudoPatternGroup(
                    apiCategory: pseudoCategory,
                    id: "pseudo.pseudo_total_life",
                    exception: new Regex("Zombies|Transformed"),
                    new PseudoPattern(new Regex("to maximum Life$")),
                    new PseudoPattern(new Regex("to Strength$"), 0.5),
                    new PseudoPattern(new Regex("(?=.*Strength)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$"), 0.5),
                    new PseudoPattern(new Regex("to all Attributes$"), 0.5)),

                // +# total maximum Mana
                new PseudoPatternGroup(
                    apiCategory: pseudoCategory,
                    id: "pseudo.pseudo_total_mana",
                    exception: new Regex("Transformed"),
                    new PseudoPattern(new Regex("to maximum Mana$")),
                    new PseudoPattern(new Regex("to Intelligence$"), 0.5),
                    new PseudoPattern(new Regex("(?=.*Intelligence)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$"), 0.5),
                    new PseudoPattern(new Regex("to all Attributes$"), 0.5)),

                // +# total maximum Energy Shield
                new PseudoPatternGroup(
                    apiCategory: pseudoCategory,
                    id: "pseudo.pseudo_total_energy_shield",
                    new PseudoPattern(new Regex("to maximum Energy Shield$"))),

                // #% total increased maximum Energy Shield
                new PseudoPatternGroup(
                    apiCategory: pseudoCategory,
                    id: "pseudo.pseudo_increased_energy_shield",
                    new PseudoPattern(new Regex("% increased maximum Energy Shield$"))),

                // +#% total Attack Speed
                new PseudoPatternGroup(
                    apiCategory: pseudoCategory,
                    id: "pseudo.pseudo_total_attack_speed",
                    new PseudoPattern(new Regex("^\\#% increased Attack Speed$"))),

                // +#% total Cast Speed
                new PseudoPatternGroup(
                    apiCategory: pseudoCategory,
                    id: "pseudo.pseudo_total_cast_speed",
                    new PseudoPattern(new Regex("^\\#% increased Cast Speed$"))),

                // #% increased Movement Speed
                new PseudoPatternGroup(
                    apiCategory: pseudoCategory,
                    id: "pseudo.pseudo_increased_movement_speed",
                    new PseudoPattern(new Regex("^\\#% increased Movement Speed$"))),

                // #% total increased Physical Damage
                // pseudo.pseudo_increased_physical_damage

                // +#% Global Critical Strike Chance
                // pseudo.pseudo_global_critical_strike_chance

                // +#% total Critical Strike Chance for Spells
                // pseudo.pseudo_critical_strike_chance_for_spells

                // +#% Global Critical Strike Multiplier
                // pseudo.pseudo_global_critical_strike_multiplier

                // Adds # to # Physical Damage
                // pseudo.pseudo_adds_physical_damage

                // Adds # to # Lightning Damage
                // pseudo.pseudo_adds_lightning_damage

                // Adds # to # Cold Damage
                // pseudo.pseudo_adds_cold_damage

                // Adds # to # Fire Damage
                // pseudo.pseudo_adds_fire_damage

                // Adds # to # Elemental Damage
                // pseudo.pseudo_adds_elemental_damage

                // Adds # to # Chaos Damage
                // pseudo.pseudo_adds_chaos_damage

                // Adds # to # Damage
                // pseudo.pseudo_adds_damage

                // Adds # to # Physical Damage to Attacks
                // pseudo.pseudo_adds_physical_damage_to_attacks

                // Adds # to # Lightning Damage to Attacks
                // pseudo.pseudo_adds_lightning_damage_to_attacks

                // Adds # to # Cold Damage to Attacks
                // pseudo.pseudo_adds_cold_damage_to_attacks

                // Adds # to # Fire Damage to Attacks
                // pseudo.pseudo_adds_fire_damage_to_attacks

                // Adds # to # Elemental Damage to Attacks
                // pseudo.pseudo_adds_elemental_damage_to_attacks

                // Adds # to # Chaos Damage to Attacks
                // pseudo.pseudo_adds_chaos_damage_to_attacks

                // Adds # to # Damage to Attacks
                // pseudo.pseudo_adds_damage_to_attacks

                // Adds # to # Physical Damage to Spells
                // pseudo.pseudo_adds_physical_damage_to_spells

                // Adds # to # Lightning Damage to Spells
                // pseudo.pseudo_adds_lightning_damage_to_spells

                // Adds # to # Cold Damage to Spells
                // pseudo.pseudo_adds_cold_damage_to_spells

                // Adds # to # Fire Damage to Spells
                // pseudo.pseudo_adds_fire_damage_to_spells

                // Adds # to # Elemental Damage to Spells
                // pseudo.pseudo_adds_elemental_damage_to_spells

                // Adds # to # Chaos Damage to Spells
                // pseudo.pseudo_adds_chaos_damage_to_spells

                // Adds # to # Damage to Spells
                // pseudo.pseudo_adds_damage_to_spells

                // #% increased Elemental Damage
                // pseudo.pseudo_increased_elemental_damage

                // #% increased Lightning Damage
                // pseudo.pseudo_increased_lightning_damage

                // #% increased Cold Damage
                // pseudo.pseudo_increased_cold_damage

                // #% increased Fire Damage
                // pseudo.pseudo_increased_fire_damage

                // #% increased Spell Damage
                // pseudo.pseudo_increased_spell_damage

                // #% increased Lightning Spell Damage
                // pseudo.pseudo_increased_lightning_spell_damage

                // #% increased Cold Spell Damage
                // pseudo.pseudo_increased_cold_spell_damage

                // #% increased Fire Spell Damage
                // pseudo.pseudo_increased_fire_spell_damage

                // #% increased Lightning Damage with Attack Skills
                // pseudo.pseudo_increased_lightning_damage_with_attack_skills

                // #% increased Cold Damage with Attack Skills
                // pseudo.pseudo_increased_cold_damage_with_attack_skills

                // #% increased Fire Damage with Attack Skills
                // pseudo.pseudo_increased_fire_damage_with_attack_skills

                // #% increased Elemental Damage with Attack Skills
                // pseudo.pseudo_increased_elemental_damage_with_attack_skills

                // #% increased Rarity of Items found
                // pseudo.pseudo_increased_rarity

                // #% increased Burning Damage
                // pseudo.pseudo_increased_burning_damage

                // # Life Regenerated per Second
                // pseudo.pseudo_total_life_regen

                // #% of Life Regenerated per Second
                // pseudo.pseudo_percent_life_regen

                // #% of Physical Attack Damage Leeched as Life
                // pseudo.pseudo_physical_attack_damage_leeched_as_life

                // #% of Physical Attack Damage Leeched as Mana
                // pseudo.pseudo_physical_attack_damage_leeched_as_mana

                // #% increased Mana Regeneration Rate
                // pseudo.pseudo_increased_mana_regen

                // +# total to Level of Socketed Gems
                // pseudo.pseudo_total_additional_gem_levels

                // +# total to Level of Socketed Elemental Gems
                // pseudo.pseudo_total_additional_elemental_gem_levels

                // +# total to Level of Socketed Fire Gems
                // pseudo.pseudo_total_additional_fire_gem_levels

                // +# total to Level of Socketed Cold Gems
                // pseudo.pseudo_total_additional_cold_gem_levels

                // +# total to Level of Socketed Lightning Gems
                // pseudo.pseudo_total_additional_lightning_gem_levels

                // +# total to Level of Socketed Chaos Gems
                // pseudo.pseudo_total_additional_chaos_gem_levels

                // +# total to Level of Socketed Spell Gems
                // pseudo.pseudo_total_additional_spell_gem_levels

                // +# total to Level of Socketed Projectile Gems
                // pseudo.pseudo_total_additional_projectile_gem_levels

                // +# total to Level of Socketed Bow Gems
                // pseudo.pseudo_total_additional_bow_gem_levels

                // +# total to Level of Socketed Melee Gems
                // pseudo.pseudo_total_additional_melee_gem_levels

                // +# total to Level of Socketed Minion Gems
                // pseudo.pseudo_total_additional_minion_gem_levels

                // +# total to Level of Socketed Strength Gems
                // pseudo.pseudo_total_additional_strength_gem_levels

                // +# total to Level of Socketed Dexterity Gems
                // pseudo.pseudo_total_additional_dexterity_gem_levels

                // +# total to Level of Socketed Intelligence Gems
                // pseudo.pseudo_total_additional_intelligence_gem_levels

                // +# total to Level of Socketed Aura Gems
                // pseudo.pseudo_total_additional_aura_gem_levels

                // +# total to Level of Socketed Movement Gems
                // pseudo.pseudo_total_additional_movement_gem_levels

                // +# total to Level of Socketed Curse Gems
                // pseudo.pseudo_total_additional_curse_gem_levels

                // +# total to Level of Socketed Vaal Gems
                // pseudo.pseudo_total_additional_vaal_gem_levels

                // +# total to Level of Socketed Support Gems
                // pseudo.pseudo_total_additional_support_gem_levels

                // +# total to Level of Socketed Skill Gems
                // pseudo.pseudo_total_additional_skill_gem_levels

                // +# total to Level of Socketed Warcry Gems
                // pseudo.pseudo_total_additional_warcry_gem_levels

                // +# total to Level of Socketed Golem Gems
                // pseudo.pseudo_total_additional_golem_gem_levels

                // # Implicit Modifiers
                // pseudo.pseudo_number_of_implicit_mods

                // # Prefix Modifiers
                // pseudo.pseudo_number_of_prefix_mods

                // # Suffix Modifiers
                // pseudo.pseudo_number_of_suffix_mods

                // # Modifiers
                // pseudo.pseudo_number_of_affix_mods

                // # Crafted Prefix Modifiers
                // pseudo.pseudo_number_of_crafted_prefix_mods

                // # Crafted Suffix Modifiers
                // pseudo.pseudo_number_of_crafted_suffix_mods

                // # Crafted Modifiers
                // pseudo.pseudo_number_of_crafted_mods

                // # Empty Prefix Modifiers
                // pseudo.pseudo_number_of_empty_prefix_mods

                // # Empty Suffix Modifiers
                // pseudo.pseudo_number_of_empty_suffix_mods

                // # Empty Modifiers
                // pseudo.pseudo_number_of_empty_affix_mods

                // # Incubator Kills (Whispering)
                // pseudo.pseudo_whispering_incubator_kills

                // # Incubator Kills (Fine)
                // pseudo.pseudo_fine_incubator_kills

                // # Incubator Kills (Singular)
                // pseudo.pseudo_singular_incubator_kills

                // # Incubator Kills (Cartographer's)
                // pseudo.pseudo_cartographers_incubator_kills

                // # Incubator Kills (Otherwordly)
                // pseudo.pseudo_otherworldly_incubator_kills

                // # Incubator Kills (Abyssal)
                // pseudo.pseudo_abyssal_incubator_kills

                // # Incubator Kills (Fragmented)
                // pseudo.pseudo_fragmented_incubator_kills

                // # Incubator Kills (Skittering)
                // pseudo.pseudo_skittering_incubator_kills

                // # Incubator Kills (Infused)
                // pseudo.pseudo_infused_incubator_kills

                // # Incubator Kills (Fossilised)
                // pseudo.pseudo_fossilised_incubator_kills

                // # Incubator Kills (Decadent)
                // pseudo.pseudo_decadent_incubator_kills

                // # Incubator Kills (Diviner's)
                // pseudo.pseudo_diviners_incubator_kills

                // # Incubator Kills (Primal)
                // pseudo.pseudo_primal_incubator_kills

                // # Incubator Kills (Enchanted)
                // pseudo.pseudo_enchanted_incubator_kills

                // # Incubator Kills (Geomancer's)
                // pseudo.pseudo_geomancers_incubator_kills

                // # Incubator Kills (Ornate)
                // pseudo.pseudo_ornate_incubator_kills

                // # Incubator Kills (Time-Lost)
                // pseudo.pseudo_timelost_incubator_kills

                // # Incubator Kills (Celestial Armoursmith's)
                // pseudo.pseudo_celestial_armoursmiths_incubator_kills

                // # Incubator Kills (Celestial Blacksmith's)
                // pseudo.pseudo_celestial_blacksmiths_incubator_kills

                // # Incubator Kills (Celestial Jeweller's)
                // pseudo.pseudo_celestial_jewellers_incubator_kills

                // # Incubator Kills (Eldritch)
                // pseudo.pseudo_eldritch_incubator_kills

                // # Incubator Kills (Obscured)
                // pseudo.pseudo_obscured_incubator_kills

                // # Incubator Kills (Foreboding)
                // pseudo.pseudo_foreboding_incubator_kills

                // # Incubator Kills (Thaumaturge's)
                // pseudo.pseudo_thaumaturges_incubator_kills

                // # Incubator Kills (Mysterious)
                // pseudo.pseudo_mysterious_incubator_kills

                // # Incubator Kills (Gemcutter's)
                // pseudo.pseudo_gemcutters_incubator_kills

                // # Incubator Kills (Feral)
                // pseudo.pseudo_feral_incubator_kills

                // # Fractured Modifiers
                // pseudo.pseudo_number_of_fractured_mods

                // +#% Quality to Elemental Damage Modifiers
                // pseudo.pseudo_jewellery_elemental_quality

                // +#% Quality to Caster Modifiers
                // pseudo.pseudo_jewellery_caster_quality

                // +#% Quality to Attack Modifiers
                // pseudo.pseudo_jewellery_attack_quality

                // +#% Quality to Defence Modifiers
                // pseudo.pseudo_jewellery_defense_quality

                // +#% Quality to Life and Mana Modifiers
                // pseudo.pseudo_jewellery_resource_quality

                // +#% Quality to Resistance Modifiers
                // pseudo.pseudo_jewellery_resistance_quality

                // +#% Quality to Attribute Modifiers
                // pseudo.pseudo_jewellery_attribute_quality
            };

            return groups;
        }

        public List<Modifier> Parse(List<ModifierLine> modifiers)
        {
            var pseudo = new List<Modifier>();

            foreach (var line in modifiers)
            {
                if (line.Modifier == null)
                {
                    continue;
                }

                FillPseudo(pseudo, line.Modifier);
            }

            pseudo.ForEach(x =>
            {
                if (x.Text == null)
                {
                    return;
                }

                x.Text = ParseHashPattern.Replace(x.Text, ((int)x.Values[0]).ToString(), 1);
            });

            return pseudo;
        }

        private void FillPseudo(List<Modifier> pseudoMods, Modifier modifier)
        {
            Modifier? pseudoMod;
            foreach (var pseudoDefinition in Definitions)
            {
                foreach (var pseudoModifier in pseudoDefinition.Modifiers)
                {
                    if (pseudoModifier.Ids.Any(id => id == modifier.Id))
                    {
                        pseudoMod = pseudoMods.FirstOrDefault(x => x.Id == pseudoDefinition.Id);
                        if (pseudoMod == null)
                        {
                            pseudoMod = new Modifier(
                                text: pseudoDefinition.Text)
                            {
                                Id = pseudoDefinition.Id,
                                Category = ModifierCategory.Pseudo,
                            };
                            pseudoMod.Values.Add((int)(modifier.Values.FirstOrDefault() * pseudoModifier.Multiplier));
                            pseudoMods.Add(pseudoMod);
                        }
                        else
                        {
                            pseudoMod.Values[0] += (int)(modifier.Values.FirstOrDefault() * pseudoModifier.Multiplier);
                        }

                        break;
                    }
                }
            }
        }
    }
}
