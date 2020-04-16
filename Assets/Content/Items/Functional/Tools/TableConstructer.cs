﻿using UnityEngine;
using SS3D.Engine.Tiles;
using SS3D.Engine.Interactions;

namespace SS3D.Content.Items.Functional.Tools
{
    /**
     * Constructs and deconstructs tables
     * 
     * <inheritdoc cref="Core.Interaction" />
     */
    public class TableConstructer : MonoBehaviour, Interaction
    {
        [SerializeField]
        private Fixture tableToConstruct = null;

        public InteractionEvent Event { get; set; }
        public string Name => ShouldDeconstruct ? "Deconstruct Table" : "Construct Table";

        // The distance in which to allow constructing tables.
        public float buildDistance = 3f;

        public bool CanInteract()
        {
            targetTile = Event.target.GetComponentInParent<TileObject>();

            // Note: I didn't write the failure conditions over here, just rewrote in a different way.
            // Not quite sure what the second one does.

            // If target tile exists.
            if (targetTile == null)
            {
                return false;
            }

            if (Event.tool != gameObject)
            {
                return false;
            }

            // Make sure there's not a wall on the turf.
            if (targetTile.Tile.turf?.isWall == true)
            {
                return false;
            }

            var player = transform.root;
            if (player != gameObject)
            {
                if (Vector3.Distance(player.transform.position, targetTile.transform.position) > buildDistance)
                {
                    return false;
                }
            }

            return true;
        }

        public void Interact()
        {
            // Note: CanInteract is always called before Interact, so we KNOW targetTile is defined.
            var tileManager = FindObjectOfType<TileManager>();

            var tile = targetTile.Tile;

            if (tile.fixture != null) // If there is a fixture on the place
            {
                if (tile.fixture == tableToConstruct) // If the fixture is a table
                {
                    tile.fixture = null; // Deconstruct
                }
            }
            else // If there is no fixture on place
            {
                tile.fixture = tableToConstruct; // Construct
            }

            // TODO: Make an easier way of doing this.
            tile.subStates = new object[2];
            tile.subStates[0] = tile.subStates?[0] ?? null;
            tile.subStates[1] = null;

            tileManager.UpdateTile(targetTile.transform.position, tile);
        }

        bool ShouldDeconstruct => targetTile.Tile.fixture == tableToConstruct;
        TileObject targetTile;
    }
}