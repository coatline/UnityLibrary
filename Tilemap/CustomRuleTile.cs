using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class CustomRuleTile : RuleTile<CustomRuleTile> {
    //public bool customField;

    //public class Neighbor : RuleTile.TilingRule.Neighbor {
    //    public const int Null = 3;
    //    public const int NotNull = 4;
    //}

    //public override bool RuleMatch(int neighbor, TileBase tile) {
    //    switch (neighbor) {
    //        case Neighbor.Null: return tile == null;
    //        case Neighbor.NotNull: return tile != null;
    //    }
    //    return base.RuleMatch(neighbor, tile);
    //}

    public string type;

    public override bool RuleMatch(int neighbor, TileBase other)
    {
        if (other is RuleOverrideTile)
            other = (other as RuleOverrideTile).m_InstanceTile;

        CustomRuleTile otherTile = other as CustomRuleTile;

        if (otherTile == null)
            return base.RuleMatch(neighbor, other);

        switch (neighbor)
        {
            case TilingRule.Neighbor.This: return type == otherTile.type;
            case TilingRule.Neighbor.NotThis: return type != otherTile.type;
        }
        return true;
    }
}