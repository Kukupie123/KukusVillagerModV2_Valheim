using System;
using UnityEngine;

// Token: 0x0200000C RID: 12

//GANE CODE, KEPT FOR REFERENCE
public class CreatureSpawner : MonoBehaviour
{
	// Token: 0x0600011A RID: 282 RVA: 0x00008065 File Offset: 0x00006265
	private void Awake()
	{
		this.m_nview = base.GetComponent<ZNetView>();
		if (this.m_nview.GetZDO() == null)
		{
			return;
		}
		base.InvokeRepeating("UpdateSpawner", UnityEngine.Random.Range(3f, 5f), 5f);
	}

	// Token: 0x0600011B RID: 283 RVA: 0x000080A0 File Offset: 0x000062A0
	private void UpdateSpawner()
	{
		if (!this.m_nview.IsOwner())
		{
			return;
		}
		ZDOID zdoid = this.m_nview.GetZDO().GetZDOID("spawn_id");
		if (this.m_respawnTimeMinuts <= 0f && !zdoid.IsNone())
		{
			return;
		}
		if (!zdoid.IsNone() && ZDOMan.instance.GetZDO(zdoid) != null)
		{
			this.m_nview.GetZDO().Set("alive_time", ZNet.instance.GetTime().Ticks);
			return;
		}
		if (this.m_respawnTimeMinuts > 0f)
		{
			DateTime time = ZNet.instance.GetTime();
			DateTime d = new DateTime(this.m_nview.GetZDO().GetLong("alive_time", 0L));
			if ((time - d).TotalMinutes < (double)this.m_respawnTimeMinuts)
			{
				return;
			}
		}
		if (!this.m_spawnAtDay && EnvMan.instance.IsDay())
		{
			return;
		}
		if (!this.m_spawnAtNight && EnvMan.instance.IsNight())
		{
			return;
		}
		bool requireSpawnArea = this.m_requireSpawnArea;
		if (!this.m_spawnInPlayerBase && EffectArea.IsPointInsideArea(base.transform.position, EffectArea.Type.PlayerBase, 0f))
		{
			return;
		}
		if (this.m_triggerNoise > 0f)
		{
			if (!Player.IsPlayerInRange(base.transform.position, this.m_triggerDistance, this.m_triggerNoise))
			{
				return;
			}
		}
		else if (!Player.IsPlayerInRange(base.transform.position, this.m_triggerDistance))
		{
			return;
		}
		this.Spawn();
	}

	// Token: 0x0600011C RID: 284 RVA: 0x00008218 File Offset: 0x00006418
	private bool HasSpawned()
	{
		return !(this.m_nview == null) && this.m_nview.GetZDO() != null && !this.m_nview.GetZDO().GetZDOID("spawn_id").IsNone();
	}

	// Token: 0x0600011D RID: 285 RVA: 0x00008264 File Offset: 0x00006464
	private ZNetView Spawn()
	{
		Vector3 position = base.transform.position;
		float y;
		if (ZoneSystem.instance.FindFloor(position, out y))
		{
			position.y = y;
		}
		Quaternion rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_creaturePrefab, position, rotation);
		ZNetView component = gameObject.GetComponent<ZNetView>();
		BaseAI component2 = gameObject.GetComponent<BaseAI>();
		if (component2 != null && this.m_setPatrolSpawnPoint)
		{
			component2.SetPatrolPoint();
		}
		if (this.m_maxLevel > 1)
		{
			Character component3 = gameObject.GetComponent<Character>();
			if (component3)
			{
				int num = this.m_minLevel;
				while (num < this.m_maxLevel && UnityEngine.Random.Range(0f, 100f) <= this.m_levelupChance)
				{
					num++;
				}
				if (num > 1)
				{
					component3.SetLevel(num);
				}
			}
		}
		component.GetZDO().SetPGWVersion(this.m_nview.GetZDO().GetPGWVersion());
		this.m_nview.GetZDO().Set("spawn_id", component.GetZDO().m_uid);
		this.m_nview.GetZDO().Set("alive_time", ZNet.instance.GetTime().Ticks);
		this.SpawnEffect(gameObject);
		return component;
	}

	// Token: 0x0600011E RID: 286 RVA: 0x000083B4 File Offset: 0x000065B4
	private void SpawnEffect(GameObject spawnedObject)
	{
		Character component = spawnedObject.GetComponent<Character>();
		Vector3 basePos = component ? component.GetCenterPoint() : (base.transform.position + Vector3.up * 0.75f);
		this.m_spawnEffects.Create(basePos, Quaternion.identity, null, 1f, -1);
	}

	// Token: 0x0600011F RID: 287 RVA: 0x00008411 File Offset: 0x00006611
	private float GetRadius()
	{
		return 0.75f;
	}

	// Token: 0x06000120 RID: 288 RVA: 0x00002971 File Offset: 0x00000B71
	private void OnDrawGizmos()
	{
	}

	// Token: 0x04000100 RID: 256
	private const float m_radius = 0.75f;

	// Token: 0x04000101 RID: 257
	public GameObject m_creaturePrefab;

	// Token: 0x04000102 RID: 258
	[Header("Level")]
	public int m_maxLevel = 1;

	// Token: 0x04000103 RID: 259
	public int m_minLevel = 1;

	// Token: 0x04000104 RID: 260
	public float m_levelupChance = 10f;

	// Token: 0x04000105 RID: 261
	[Header("Spawn settings")]
	public float m_respawnTimeMinuts = 20f;

	// Token: 0x04000106 RID: 262
	public float m_triggerDistance = 60f;

	// Token: 0x04000107 RID: 263
	public float m_triggerNoise;

	// Token: 0x04000108 RID: 264
	public bool m_spawnAtNight = true;

	// Token: 0x04000109 RID: 265
	public bool m_spawnAtDay = true;

	// Token: 0x0400010A RID: 266
	public bool m_requireSpawnArea;

	// Token: 0x0400010B RID: 267
	public bool m_spawnInPlayerBase;

	// Token: 0x0400010C RID: 268
	public bool m_setPatrolSpawnPoint;

	// Token: 0x0400010D RID: 269
	public EffectList m_spawnEffects = new EffectList();

	// Token: 0x0400010E RID: 270
	private ZNetView m_nview;
}
