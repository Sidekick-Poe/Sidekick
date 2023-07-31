using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sidekick.Apis.Poe.Modifiers;
using Sidekick.Apis.Poe.Modifiers.Models;
using Sidekick.Apis.Poe.Pseudo.Models;
using Sidekick.Common.Game.Items.Modifiers;

namespace Sidekick.Apis.Poe.Pseudo
{
    public class PseudoModifierProvider : IPseudoModifierProvider
    {
        private readonly Regex ParseHashPattern = new("\\#");

        private readonly ILogger<PseudoModifierProvider> logger;
        private readonly IEnglishModifierProvider englishModifierProvider;

        public PseudoModifierProvider(
            ILogger<PseudoModifierProvider> logger,
            IEnglishModifierProvider englishModifierProvider)
        {
            this.logger = logger;
            this.englishModifierProvider = englishModifierProvider;
        }

        private List<PseudoDefinition> Definitions { get; set; } = new List<PseudoDefinition>();

        public async Task Initialize()
        {
            if (Definitions != null && Definitions.Count > 0)
            {
                return;
            }

            try
            {
                logger.LogInformation($"Pseudo stat service initialization started.");

                var result = await englishModifierProvider.GetList();

                logger.LogInformation($"{result.Count} attributes fetched.");

                var groups = InitGroups(result);

                foreach (var category in result)
                {
                    var first = category.Entries.FirstOrDefault();
                    if (first == null || first.Id.Split('.').First() == "pseudo")
                    {
                        continue;
                    }

                    foreach (var entry in category.Entries)
                    {
                        foreach (var group in groups)
                        {
                            if (group.Exception != null && group.Exception.IsMatch(entry.Text))
                            {
                                continue;
                            }

                            foreach (var pattern in group.Patterns)
                            {
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
                    var definition = new PseudoDefinition(group.Id, group.Text);

                    foreach (var pattern in group.Patterns)
                    {
                        PseudoDefinitionModifier modifier = null;

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
            catch (Exception e)
            {
                logger.LogWarning(e, "Could not initialize pseudo service.");
            }
        }

        private static List<PseudoPatternGroup> InitGroups(List<ApiCategory> categories)
        {
            var groups = new List<PseudoPatternGroup>() {
                // +#% total to Cold Resistance
                new PseudoPatternGroup("pseudo.pseudo_total_cold_resistance", new Regex("Minions|Enemies|Totems"), new List<PseudoPattern>(){
                    new PseudoPattern(new Regex("to Cold Resistance$")),
                    new PseudoPattern(new Regex("(?=.*Cold)to (?:Fire|Cold|Lightning|Chaos) and (?:Fire|Cold|Lightning|Chaos) Resistances$")),
                }),
                // +#% total to Fire Resistance
                new PseudoPatternGroup("pseudo.pseudo_total_fire_resistance", new Regex("Minions|Enemies|Totems"), new List<PseudoPattern>(){
                    new PseudoPattern(new Regex("to Fire Resistance$")),
                    new PseudoPattern(new Regex("(?=.*Fire)to (?:Fire|Cold|Lightning|Chaos) and (?:Fire|Cold|Lightning|Chaos) Resistances$")),
                }),
                // +#% total to Lightning Resistance
                new PseudoPatternGroup("pseudo.pseudo_total_lightning_resistance", new Regex("Minions|Enemies|Totems"), new List<PseudoPattern>(){
                    new PseudoPattern(new Regex("to Lightning Resistance$")),
                    new PseudoPattern(new Regex("(?=.*Lightning)to (?:Fire|Cold|Lightning|Chaos) and (?:Fire|Cold|Lightning|Chaos) Resistances$")),
                }),
                // +#% total to Chaos Resistance
                new PseudoPatternGroup("pseudo.pseudo_total_chaos_resistance", new Regex("Minions|Enemies|Totems"), new List<PseudoPattern>(){
                    new PseudoPattern(new Regex("to Chaos Resistance$")),
                    new PseudoPattern(new Regex("(?=.*Chaos)to (?:Fire|Cold|Lightning|Chaos) and (?:Fire|Cold|Lightning|Chaos) Resistances$")),
                }),
                // +#% total to all Elemental Resistances
                new PseudoPatternGroup("pseudo.pseudo_total_all_elemental_resistances", new Regex("Minions|Enemies|Totems"), new List<PseudoPattern>(){
                    new PseudoPattern(new Regex("to all Elemental Resistances$")),
                }),
                // +#% total Elemental Resistance
                new PseudoPatternGroup("pseudo.pseudo_total_elemental_resistance", new Regex("Minions|Enemies|Totems"), new List<PseudoPattern>(){
                    new PseudoPattern(new Regex("to (?:Fire|Cold|Lightning) Resistance$")),
                    new PseudoPattern(new Regex("to (?:Fire|Cold|Lightning) and (?:Fire|Cold|Lightning) Resistances$"), 2),
                    new PseudoPattern(new Regex("(?=.*Chaos)to (?:Fire|Cold|Lightning|Chaos) and (?:Fire|Cold|Lightning|Chaos) Resistances$")),
                    new PseudoPattern(new Regex("to all Elemental Resistances$"), 3),
                }),
                // +#% total Resistance
                new PseudoPatternGroup("pseudo.pseudo_total_resistance", new Regex("Minions|Enemies|Totems"), new List<PseudoPattern>(){
                    new PseudoPattern(new Regex("to (Fire|Cold|Lightning|Chaos) Resistance$")),
                    new PseudoPattern(new Regex("to (?:Fire|Cold|Lightning|Chaos) and (?:Fire|Cold|Lightning|Chaos) Resistances$"), 2),
                    new PseudoPattern(new Regex("to all Elemental Resistances$"), 3),
                }),
                //// # total Resistances
                //new PseudoPatternGroup("pseudo.pseudo_count_resistances", null, new List<PseudoPattern>(){ }),
                //// # total Elemental Resistances
                //new PseudoPatternGroup("pseudo.pseudo_count_elemental_resistances", null, new List<PseudoPattern>(){ }),
                // +# total to Strength
                new PseudoPatternGroup("pseudo.pseudo_total_strength", new Regex("Passive"), new List<PseudoPattern>(){
                    new PseudoPattern(new Regex("to Strength$")),
                    new PseudoPattern(new Regex("(?=.*Strength)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$")),
                    new PseudoPattern(new Regex("to all Attributes$")),
                }),
                // +# total to Dexterity
                new PseudoPatternGroup("pseudo.pseudo_total_dexterity", new Regex("Passive"), new List<PseudoPattern>(){
                    new PseudoPattern(new Regex("to Dexterity$")),
                    new PseudoPattern(new Regex("(?=.*Dexterity)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$")),
                    new PseudoPattern(new Regex("to all Attributes$")),
                }),
                // +# total to Intelligence
                new PseudoPatternGroup("pseudo.pseudo_total_intelligence", new Regex("Passive"), new List<PseudoPattern>(){
                    new PseudoPattern(new Regex("to Intelligence$")),
                    new PseudoPattern(new Regex("(?=.*Intelligence)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$")),
                    new PseudoPattern(new Regex("to all Attributes$")),
                }),
                // +# total to all Attributes
                new PseudoPatternGroup("pseudo.pseudo_total_all_attributes", new Regex("Passive"), new List<PseudoPattern>(){
                    new PseudoPattern(new Regex("to all Attributes$")),
                }),
                // +# total maximum Life
                new PseudoPatternGroup("pseudo.pseudo_total_life", new Regex("Zombies|Transformed"), new List<PseudoPattern>(){
                    new PseudoPattern(new Regex("to maximum Life$")),
                    new PseudoPattern(new Regex("to Strength$"), 0.5),
                    new PseudoPattern(new Regex("(?=.*Strength)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$"), 0.5),
                    new PseudoPattern(new Regex("to all Attributes$"), 0.5),
                }),
                // +# total maximum Mana
                new PseudoPatternGroup("pseudo.pseudo_total_mana", new Regex("Transformed"), new List<PseudoPattern>(){
                    new PseudoPattern(new Regex("to maximum Mana$")),
                    new PseudoPattern(new Regex("to Intelligence$"), 0.5),
                    new PseudoPattern(new Regex("(?=.*Intelligence)to (?:Strength|Dexterity|Intelligence) and (?:Strength|Dexterity|Intelligence)$"), 0.5),
                    new PseudoPattern(new Regex("to all Attributes$"), 0.5),
                }),
                // +# total maximum Energy Shield
                new PseudoPatternGroup("pseudo.pseudo_total_energy_shield", null, new List<PseudoPattern>(){
                    new PseudoPattern(new Regex("to maximum Energy Shield$")),
                }),
                // #% total increased maximum Energy Shield
                new PseudoPatternGroup("pseudo.pseudo_increased_energy_shield", null, new List<PseudoPattern>(){
                    new PseudoPattern(new Regex("% increased maximum Energy Shield$")),
                }),
                //// +#% total Attack Speed
                new PseudoPatternGroup("pseudo.pseudo_total_attack_speed", null, new List<PseudoPattern>(){
                    new PseudoPattern(new Regex("^\\#% increased Attack Speed$")),
                }),
                // +#% total Cast Speed
                new PseudoPatternGroup("pseudo.pseudo_total_cast_speed", null, new List<PseudoPattern>(){
                    new PseudoPattern(new Regex("^\\#% increased Cast Speed$")),
                }),
                // #% increased Movement Speed
                new PseudoPatternGroup("pseudo.pseudo_increased_movement_speed", null, new List<PseudoPattern>(){
                    new PseudoPattern(new Regex("^\\#% increased Movement Speed$")),
                }),
                //// #% total increased Physical Damage
                //new PseudoPatternGroup("pseudo.pseudo_increased_physical_damage", null, new List<PseudoPattern>(){ }),
                //// +#% Global Critical Strike Chance
                //new PseudoPatternGroup("pseudo.pseudo_global_critical_strike_chance", null, new List<PseudoPattern>(){ }),
                //// +#% total Critical Strike Chance for Spells
                //new PseudoPatternGroup("pseudo.pseudo_critical_strike_chance_for_spells", null, new List<PseudoPattern>(){ }),
                //// +#% Global Critical Strike Multiplier
                //new PseudoPatternGroup("pseudo.pseudo_global_critical_strike_multiplier", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Physical Damage
                //new PseudoPatternGroup("pseudo.pseudo_adds_physical_damage", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Lightning Damage
                //new PseudoPatternGroup("pseudo.pseudo_adds_lightning_damage", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Cold Damage
                //new PseudoPatternGroup("pseudo.pseudo_adds_cold_damage", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Fire Damage
                //new PseudoPatternGroup("pseudo.pseudo_adds_fire_damage", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Elemental Damage
                //new PseudoPatternGroup("pseudo.pseudo_adds_elemental_damage", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Chaos Damage
                //new PseudoPatternGroup("pseudo.pseudo_adds_chaos_damage", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Damage
                //new PseudoPatternGroup("pseudo.pseudo_adds_damage", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Physical Damage to Attacks
                //new PseudoPatternGroup("pseudo.pseudo_adds_physical_damage_to_attacks", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Lightning Damage to Attacks
                //new PseudoPatternGroup("pseudo.pseudo_adds_lightning_damage_to_attacks", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Cold Damage to Attacks
                //new PseudoPatternGroup("pseudo.pseudo_adds_cold_damage_to_attacks", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Fire Damage to Attacks
                //new PseudoPatternGroup("pseudo.pseudo_adds_fire_damage_to_attacks", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Elemental Damage to Attacks
                //new PseudoPatternGroup("pseudo.pseudo_adds_elemental_damage_to_attacks", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Chaos Damage to Attacks
                //new PseudoPatternGroup("pseudo.pseudo_adds_chaos_damage_to_attacks", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Damage to Attacks
                //new PseudoPatternGroup("pseudo.pseudo_adds_damage_to_attacks", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Physical Damage to Spells
                //new PseudoPatternGroup("pseudo.pseudo_adds_physical_damage_to_spells", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Lightning Damage to Spells
                //new PseudoPatternGroup("pseudo.pseudo_adds_lightning_damage_to_spells", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Cold Damage to Spells
                //new PseudoPatternGroup("pseudo.pseudo_adds_cold_damage_to_spells", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Fire Damage to Spells
                //new PseudoPatternGroup("pseudo.pseudo_adds_fire_damage_to_spells", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Elemental Damage to Spells
                //new PseudoPatternGroup("pseudo.pseudo_adds_elemental_damage_to_spells", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Chaos Damage to Spells
                //new PseudoPatternGroup("pseudo.pseudo_adds_chaos_damage_to_spells", null, new List<PseudoPattern>(){ }),
                //// Adds # to # Damage to Spells
                //new PseudoPatternGroup("pseudo.pseudo_adds_damage_to_spells", null, new List<PseudoPattern>(){ }),
                //// #% increased Elemental Damage
                //new PseudoPatternGroup("pseudo.pseudo_increased_elemental_damage", null, new List<PseudoPattern>(){ }),
                //// #% increased Lightning Damage
                //new PseudoPatternGroup("pseudo.pseudo_increased_lightning_damage", null, new List<PseudoPattern>(){ }),
                //// #% increased Cold Damage
                //new PseudoPatternGroup("pseudo.pseudo_increased_cold_damage", null, new List<PseudoPattern>(){ }),
                //// #% increased Fire Damage
                //new PseudoPatternGroup("pseudo.pseudo_increased_fire_damage", null, new List<PseudoPattern>(){ }),
                //// #% increased Spell Damage
                //new PseudoPatternGroup("pseudo.pseudo_increased_spell_damage", null, new List<PseudoPattern>(){ }),
                //// #% increased Lightning Spell Damage
                //new PseudoPatternGroup("pseudo.pseudo_increased_lightning_spell_damage", null, new List<PseudoPattern>(){ }),
                //// #% increased Cold Spell Damage
                //new PseudoPatternGroup("pseudo.pseudo_increased_cold_spell_damage", null, new List<PseudoPattern>(){ }),
                //// #% increased Fire Spell Damage
                //new PseudoPatternGroup("pseudo.pseudo_increased_fire_spell_damage", null, new List<PseudoPattern>(){ }),
                //// #% increased Lightning Damage with Attack Skills
                //new PseudoPatternGroup("pseudo.pseudo_increased_lightning_damage_with_attack_skills", null, new List<PseudoPattern>(){ }),
                //// #% increased Cold Damage with Attack Skills
                //new PseudoPatternGroup("pseudo.pseudo_increased_cold_damage_with_attack_skills", null, new List<PseudoPattern>(){ }),
                //// #% increased Fire Damage with Attack Skills
                //new PseudoPatternGroup("pseudo.pseudo_increased_fire_damage_with_attack_skills", null, new List<PseudoPattern>(){ }),
                //// #% increased Elemental Damage with Attack Skills
                //new PseudoPatternGroup("pseudo.pseudo_increased_elemental_damage_with_attack_skills", null, new List<PseudoPattern>(){ }),
                //// #% increased Rarity of Items found
                //new PseudoPatternGroup("pseudo.pseudo_increased_rarity", null, new List<PseudoPattern>(){ }),
                //// #% increased Burning Damage
                //new PseudoPatternGroup("pseudo.pseudo_increased_burning_damage", null, new List<PseudoPattern>(){ }),
                //// # Life Regenerated per Second
                //new PseudoPatternGroup("pseudo.pseudo_total_life_regen", null, new List<PseudoPattern>(){ }),
                //// #% of Life Regenerated per Second
                //new PseudoPatternGroup("pseudo.pseudo_percent_life_regen", null, new List<PseudoPattern>(){ }),
                //// #% of Physical Attack Damage Leeched as Life
                //new PseudoPatternGroup("pseudo.pseudo_physical_attack_damage_leeched_as_life", null, new List<PseudoPattern>(){ }),
                //// #% of Physical Attack Damage Leeched as Mana
                //new PseudoPatternGroup("pseudo.pseudo_physical_attack_damage_leeched_as_mana", null, new List<PseudoPattern>(){ }),
                //// #% increased Mana Regeneration Rate
                //new PseudoPatternGroup("pseudo.pseudo_increased_mana_regen", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Elemental Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_elemental_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Fire Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_fire_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Cold Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_cold_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Lightning Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_lightning_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Chaos Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_chaos_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Spell Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_spell_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Projectile Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_projectile_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Bow Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_bow_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Melee Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_melee_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Minion Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_minion_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Strength Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_strength_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Dexterity Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_dexterity_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Intelligence Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_intelligence_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Aura Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_aura_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Movement Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_movement_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Curse Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_curse_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Vaal Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_vaal_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Support Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_support_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Skill Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_skill_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Warcry Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_warcry_gem_levels", null, new List<PseudoPattern>(){ }),
                //// +# total to Level of Socketed Golem Gems
                //new PseudoPatternGroup("pseudo.pseudo_total_additional_golem_gem_levels", null, new List<PseudoPattern>(){ }),
                //// # Implicit Modifiers
                //new PseudoPatternGroup("pseudo.pseudo_number_of_implicit_mods", null, new List<PseudoPattern>(){ }),
                //// # Prefix Modifiers
                //new PseudoPatternGroup("pseudo.pseudo_number_of_prefix_mods", null, new List<PseudoPattern>(){ }),
                //// # Suffix Modifiers
                //new PseudoPatternGroup("pseudo.pseudo_number_of_suffix_mods", null, new List<PseudoPattern>(){ }),
                //// # Modifiers
                //new PseudoPatternGroup("pseudo.pseudo_number_of_affix_mods", null, new List<PseudoPattern>(){ }),
                //// # Crafted Prefix Modifiers
                //new PseudoPatternGroup("pseudo.pseudo_number_of_crafted_prefix_mods", null, new List<PseudoPattern>(){ }),
                //// # Crafted Suffix Modifiers
                //new PseudoPatternGroup("pseudo.pseudo_number_of_crafted_suffix_mods", null, new List<PseudoPattern>(){ }),
                //// # Crafted Modifiers
                //new PseudoPatternGroup("pseudo.pseudo_number_of_crafted_mods", null, new List<PseudoPattern>(){ }),
                //// # Empty Prefix Modifiers
                //new PseudoPatternGroup("pseudo.pseudo_number_of_empty_prefix_mods", null, new List<PseudoPattern>(){ }),
                //// # Empty Suffix Modifiers
                //new PseudoPatternGroup("pseudo.pseudo_number_of_empty_suffix_mods", null, new List<PseudoPattern>(){ }),
                //// # Empty Modifiers
                //new PseudoPatternGroup("pseudo.pseudo_number_of_empty_affix_mods", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Whispering)
                //new PseudoPatternGroup("pseudo.pseudo_whispering_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Fine)
                //new PseudoPatternGroup("pseudo.pseudo_fine_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Singular)
                //new PseudoPatternGroup("pseudo.pseudo_singular_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Cartographer's)
                //new PseudoPatternGroup("pseudo.pseudo_cartographers_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Otherwordly)
                //new PseudoPatternGroup("pseudo.pseudo_otherworldly_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Abyssal)
                //new PseudoPatternGroup("pseudo.pseudo_abyssal_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Fragmented)
                //new PseudoPatternGroup("pseudo.pseudo_fragmented_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Skittering)
                //new PseudoPatternGroup("pseudo.pseudo_skittering_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Infused)
                //new PseudoPatternGroup("pseudo.pseudo_infused_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Fossilised)
                //new PseudoPatternGroup("pseudo.pseudo_fossilised_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Decadent)
                //new PseudoPatternGroup("pseudo.pseudo_decadent_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Diviner's)
                //new PseudoPatternGroup("pseudo.pseudo_diviners_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Primal)
                //new PseudoPatternGroup("pseudo.pseudo_primal_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Enchanted)
                //new PseudoPatternGroup("pseudo.pseudo_enchanted_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Geomancer's)
                //new PseudoPatternGroup("pseudo.pseudo_geomancers_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Ornate)
                //new PseudoPatternGroup("pseudo.pseudo_ornate_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Time-Lost)
                //new PseudoPatternGroup("pseudo.pseudo_timelost_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Celestial Armoursmith's)
                //new PseudoPatternGroup("pseudo.pseudo_celestial_armoursmiths_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Celestial Blacksmith's)
                //new PseudoPatternGroup("pseudo.pseudo_celestial_blacksmiths_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Celestial Jeweller's)
                //new PseudoPatternGroup("pseudo.pseudo_celestial_jewellers_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Eldritch)
                //new PseudoPatternGroup("pseudo.pseudo_eldritch_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Obscured)
                //new PseudoPatternGroup("pseudo.pseudo_obscured_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Foreboding)
                //new PseudoPatternGroup("pseudo.pseudo_foreboding_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Thaumaturge's)
                //new PseudoPatternGroup("pseudo.pseudo_thaumaturges_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Mysterious)
                //new PseudoPatternGroup("pseudo.pseudo_mysterious_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Gemcutter's)
                //new PseudoPatternGroup("pseudo.pseudo_gemcutters_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Incubator Kills (Feral)
                //new PseudoPatternGroup("pseudo.pseudo_feral_incubator_kills", null, new List<PseudoPattern>(){ }),
                //// # Fractured Modifiers
                //new PseudoPatternGroup("pseudo.pseudo_number_of_fractured_mods", null, new List<PseudoPattern>(){ }),
                //// +#% Quality to Elemental Damage Modifiers
                //new PseudoPatternGroup("pseudo.pseudo_jewellery_elemental_quality", null, new List<PseudoPattern>(){ }),
                //// +#% Quality to Caster Modifiers
                //new PseudoPatternGroup("pseudo.pseudo_jewellery_caster_quality", null, new List<PseudoPattern>(){ }),
                //// +#% Quality to Attack Modifiers
                //new PseudoPatternGroup("pseudo.pseudo_jewellery_attack_quality", null, new List<PseudoPattern>(){ }),
                //// +#% Quality to Defence Modifiers
                //new PseudoPatternGroup("pseudo.pseudo_jewellery_defense_quality", null, new List<PseudoPattern>(){ }),
                //// +#% Quality to Life and Mana Modifiers
                //new PseudoPatternGroup("pseudo.pseudo_jewellery_resource_quality", null, new List<PseudoPattern>(){ }),
                //// +#% Quality to Resistance Modifiers
                //new PseudoPatternGroup("pseudo.pseudo_jewellery_resistance_quality", null, new List<PseudoPattern>(){ }),
                //// +#% Quality to Attribute Modifiers
                //new PseudoPatternGroup("pseudo.pseudo_jewellery_attribute_quality", null, new List<PseudoPattern>(){ }),
            };

            foreach (var category in categories)
            {
                var first = category.Entries.FirstOrDefault();
                if (first == null || first.Id.Split('.').First() == "pseudo")
                {
                    foreach (var entry in category.Entries)
                    {
                        var group = groups.FirstOrDefault(x => x.Id == entry.Id);
                        if (group != null)
                        {
                            group.Text = entry.Text;
                        }
                    }
                }
            }

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
                x.Text = ParseHashPattern.Replace(x.Text, ((int)x.Values[0]).ToString(), 1);
            });

            return pseudo;
        }

        private void FillPseudo(List<Modifier> pseudoMods, Modifier modifier)
        {
            Modifier pseudoMod;
            foreach (var pseudoDefinition in Definitions)
            {
                foreach (var pseudoModifier in pseudoDefinition.Modifiers)
                {
                    if (pseudoModifier.Ids.Any(id => id == modifier.Id))
                    {
                        pseudoMod = pseudoMods.FirstOrDefault(x => x.Id == pseudoDefinition.Id);
                        if (pseudoMod == null)
                        {
                            pseudoMod = new Modifier()
                            {
                                Id = pseudoDefinition.Id,
                                Text = pseudoDefinition.Text,
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
