﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
public class Item : MonoBehaviour
{
    [Header("Generation attributes")]
    public ItemRarity rarity;
    public ItemType type;
    public int minDepth;
    public int maxDepth;
    public ItemRarity elevatesTo;

    [Header("Basic item variables")]
    public int ID;
    public bool stackable;
    

    [SerializeField] Color color;

    [HideInInspector] public Vector2Int location;
    public bool held;
    private Monster heldBy;
    [SerializeField] new private string name;
    [SerializeField] private string plural;

    [HideInInspector] public bool CanEquip;
    [HideInInspector] public bool CanApply;
    [HideInInspector] public bool CanTarget;
    [HideInInspector] public bool CanMelee;
    [HideInInspector] public bool CanRanged;

    public Connections connections = new Connections();
    [HideInInspector] List<Effect> attachedEffects = new List<Effect>();

    [SerializeReference] public List<Effect> effects;


    [SerializeReference] public List<Effect> optionalEffects;


    private SpriteRenderer Render;
    public SpriteRenderer render
    {
        get 
        {
            if (Render)
            {
                return Render;
            }
            else
            {
                Render = GetComponent<SpriteRenderer>();
                Render.sortingOrder = -900;
                return Render;
            }
        }

        set
        {
            Render = value;
        }
    }

    [HideInInspector] public ApplyableItem applyable;
    [HideInInspector] public TargetableItem targetable;
    [HideInInspector] public EquipableItem equipable;
    [HideInInspector] public MeleeWeapon melee;
    [HideInInspector] public RangedWeapon ranged;

    private static readonly float itemZValue = -7.0f;

    private bool setup = false;

    //Stuff used for convenience editor hacking, and should never be seen.

    /* If you see this and don't know what this is, ask me! It's super useful
     * for hacking up the editor, and making things easy. The #if's in this file
     * are used to make the sprite in the sprite renderer equal the sprite in this file,
     * so you can't forget to not change both. */
    #if UNITY_EDITOR
    private Color currentColor;
    #endif

    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    public Item Instantiate()
    {
        Item i = Instantiate(gameObject).GetComponent<Item>();
        i.Setup();
        return i;
    }

    public void Setup()
    {
        if (setup) return;
        if (this.type == 0)
        {
            Debug.LogError("An item is set to have no type! Please use ItemType.MISC if you have misc items.", this);
        }

        applyable = GetComponent<ApplyableItem>();
        targetable = GetComponent<TargetableItem>();
        equipable = GetComponent<EquipableItem>();
        melee = GetComponent<MeleeWeapon>();
        ranged = GetComponent<RangedWeapon>();

        //Quick check for components, better here than later
        CanEquip = (equipable != null);
        CanApply = (applyable != null);
        CanTarget = (targetable != null);
        CanMelee = (melee != null);
        CanRanged = (ranged != null);

        AddEffect(effects.Select(x => x.Instantiate()).ToArray());
        setup = true;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pickup(Monster m)
    {
        heldBy = m;
        DisableSprite();
    }

    public void Drop()
    {
        heldBy = null;
    }

    public void SetLocation(Vector2Int loc)
    {
        this.location = loc;
        transform.position = new Vector3(loc.x, loc.y, itemZValue);
    }

    public void EnableSprite()
    {
        render.enabled = true;
    }

    public void DisableSprite()
    {
        render.enabled = false;
    }

    public void SetGrayscale()
    {
        float gray = color.grayscale;
        render.color = new Color(gray, gray, gray);
    }

    public void SetFullColor()
    {
        render.color = color;
    }

    public virtual void Apply()
    {

    }

    public virtual void RegenerateStats()
    {

    }

    public string GetName()
    {
        if (CanEquip && equipable.isEquipped)
        {
            return name + " [Equipped]";
        }
        return name;
    }

    //Returns the name without modifiers. As of right now, just returns the straight name.
    public string GetNameClean()
    {
        return name;
    }

    public string GetPlural()
    {
        return plural;
    }

    //TODO: Items should elevate stats as well
    public void ElevateRarityTo(ItemRarity rarity, List<Effect> extraOptions)
    {
        int numberToAdd = rarity - this.rarity;
        optionalEffects.AddRange(extraOptions.AsEnumerable());
        
        if (optionalEffects.Count < numberToAdd)
        {
            Debug.LogWarning($"{name} does not have enough options to elevate fully to rarity {rarity}! Please add more options, or mark its achievable rarity correctly.");
        }

        for (int i = 0; i < System.Math.Min(numberToAdd, optionalEffects.Count); i++)
        {
            int index = UnityEngine.Random.Range(0, optionalEffects.Count);
            AddEffect(optionalEffects[index].Instantiate());
            optionalEffects.RemoveAt(index);
            this.rarity++;
        }
    }

    public void AddEffect(params Effect[] effects)
    {
        if (connections == null) connections = new Connections(this);
        foreach (Effect e in effects)
        {
            e.Connect(connections);
            attachedEffects.Add(e);
        }
    }

    //Editor only functions - For convenience
    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (render.color != currentColor)
        {
            currentColor = render.color;
            color = render.color;
        }
        else if (color != currentColor)
        {
            currentColor = color;
            render.color = color;
        }
    }
    #endif

}
