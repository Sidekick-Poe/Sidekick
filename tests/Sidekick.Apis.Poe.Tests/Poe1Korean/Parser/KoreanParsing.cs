using Sidekick.Apis.Poe.Items;
using Sidekick.Apis.Poe.Trade;
using Xunit;

namespace Sidekick.Apis.Poe.Tests.Poe1Korean.Parser;

[Collection(Collections.Poe1KoreanFixture)]
public class KoreanParsing(Poe1KoreanFixture fixture)
{
    private readonly IItemParser parser = fixture.Parser;

    [Fact]
    public void UniqueMutatedBelt()
    {
        var actual = parser.ParseItem(@"아이템 종류: 허리띠
아이템 희귀도: 고유
삿된 말리가로의 속박
사슬 허리띠
--------
아이템 레벨: 74
--------
에너지 보호막 최대치 +15 (implicit)
--------
플레이어에게 적용되는 감전 지속시간 100% 증가
자신이 유발한 감전이 자신에게 반사됨
감전 상태일 때 피해 60% 증가
감전 상태일 때 이동 속도 15% 증가
번개 피해의 40%를 카오스 피해로 전환 (mutated)
--------
""우리의 명철이 앞으로 나아가는 길을 닦을 것이다.
 명철한 자들은 성장할 가치가 있는 법이니! """);

        Assert.Equal(ItemClass.Belt, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);

        actual.AssertHasStat(StatCategory.Mutated, "번개 피해의 #%를 카오스 피해로 전환", 40);
    }

    [Fact]
    public void KorenAmuletIntelligenceMod()
    {
        var actual = parser.ParseItem(@"아이템 종류: 목걸이
아이템 희귀도: 고유
징크스 액막이
황수정 목걸이
--------
요구사항:
레벨: 48
--------
아이템 레벨: 80
--------
힘 및 민첩 +22 (implicit)
--------
지능 +34
카오스 저항 +27%
자신의 비-저주 오라 스킬 효과 14% 증가
피격 시 받는 피해의 10%를 자신보다 자신의 망령이 먼저 받음
--------
불탄 로아의 눈과 염소인간의 수염이 명하노니
게으른 뼈는 두고 떠나라
춤춰라, 망자여. 나의 명에 따를 지어다!");

        Assert.Equal(ItemClass.Amulet, actual.Properties.ItemClass);
        Assert.Equal(Rarity.Unique, actual.Properties.Rarity);

        actual.AssertHasStat(StatCategory.Explicit, "지능 +#", 34);
    }
}
