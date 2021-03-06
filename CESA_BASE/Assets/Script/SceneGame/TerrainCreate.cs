﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCreate : MonoBehaviour
{
    public enum TerrainChild
    {
        Ground = 0, 
        Wall
    }

    [SerializeField]
    GameObject m_groundPrefab = null;
    [SerializeField]
    GameObject m_wallPrefab = null;

    /// <summary>
    /// 地面生成
    /// </summary>
    /// <param name="x">横の生成数</param>
    /// <param name="y">縦の生成数</param>
    /// <param name="underPosY">高さ座標</param>
    public void CreateGround(int width, int height, float underPosY)
    {
        Transform _ground = transform.GetChild((int)TerrainChild.Ground);
        int difference = _ground.childCount - width * height;

        // 変更前のほうが少ないもしくは同数
        if (difference <= 0)
        {
            Vector2 half = new Vector2(width / 2, height / 2);
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int idx = width * y + x;
                    // 現状存在するものの配置変更
                    if (_ground.childCount > idx)
                    {
                        _ground.GetChild(idx).position = new Vector3(x - half.x, underPosY, y - half.y);
                    }
                    else
                    {
                        GameObject　_obj = Instantiate(m_groundPrefab, new Vector3(x - half.x, underPosY, y - half.y), Quaternion.identity);
                        _obj.transform.parent = transform.GetChild((int)TerrainChild.Ground);
                        _obj.transform.tag = NameDefine.TagName.TerrainBlock;
                    }
                }
            }
        }
        // 変更後のほうが少ない
        else
        {
            // 差分を解放
            for (int i = 0; i < difference; ++i)
                Destroy(_ground.GetChild(i).gameObject);

            Vector3 half = new Vector3(width / 2, underPosY, height / 2);
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                {
                    // 現状あるもの配置変更
                    _ground.GetChild(width * y + x + difference).position = new Vector3(x - half.x, underPosY, y - half.y);
                }
        }
    }

    /// <summary>
    /// 壁生成
    /// </summary>
    public void CreateWall(int _stageSizeY)
    {
        GameFuse[] _fuseList = FindObjectsOfType<GameFuse>();
        GameGimmick[] _gimmickList = FindObjectsOfType<GameGimmick>();
        Transform _wallBefore = transform.GetChild((int)TerrainChild.Wall);
        List<Vector3> _wallAfter = new List<Vector3>();

        foreach (GameFuse _fuse in _fuseList)
        {
            // UIか地面の上に設置されているものなら
            if (_fuse.State == GameFuse.FuseState.UI ||
                _fuse.transform.position.y == - _stageSizeY / 2)
                continue;

            RaycastHit hit = new RaycastHit();
            Ray ray = new Ray(_fuse.transform.position, Vector3.down);
            int layerMask = 1 << _fuse.gameObject.layer;
            // 真下にあるオブジェクトを選択
            if (Physics.Raycast(ray, out hit, _stageSizeY, layerMask))
            {
                for (float i = hit.collider.transform.position.y + 1; i < _fuse.transform.position.y; ++i)
                {
                    _wallAfter.Add(new Vector3(_fuse.transform.position.x, i, _fuse.transform.position.z));
                }
            }
        }
        foreach (GameGimmick _fgimmick in _gimmickList)
        {
            // UIか地面の上に設置されているものなら
            if (_fgimmick.UI ||
                _fgimmick.transform.position.y == - _stageSizeY / 2)
                continue;

            RaycastHit hit = new RaycastHit();
            Ray ray = new Ray(_fgimmick.transform.position, Vector3.down);
            int layerMask = 1 << _fgimmick.gameObject.layer;
            // 真下にあるオブジェクトを選択
            if (Physics.Raycast(ray, out hit, _stageSizeY, layerMask))
            {
                for (float i = hit.collider.transform.position.y + 1; i < _fgimmick.transform.position.y; ++i)
                {
                    _wallAfter.Add(new Vector3(_fgimmick.transform.position.x, i, _fgimmick.transform.position.z));
                }
            }
        }

        int difference = _wallBefore.childCount - _wallAfter.Count;
        // 変更前のほうが少ないもしくは同数
        if (difference <= 0)
        {
            for (int i = 0; i < _wallAfter.Count; ++i)
            {
                // 現状存在するものの配置変更
                if (_wallBefore.childCount > i)
                {
                    _wallBefore.GetChild(i).position = _wallAfter[i];
                }
                else
                {
                    GameObject _obj = Instantiate(m_wallPrefab, _wallAfter[i], Quaternion.identity);
                    _obj.transform.parent = transform.GetChild((int)TerrainChild.Wall);
                    _obj.layer = NameDefine.Layer.Trans;
                }
            }
        }
        // 変更後のほうが少ない
        else
        {
            // 差分を解放
            for (int i = 0; i < difference; ++i)
                Destroy(_wallBefore.GetChild(i).gameObject);

            for (int i = 0; i < _wallAfter.Count; ++i)
            {
                // 現状あるもの配置変更
                _wallBefore.GetChild(i + difference).position = _wallAfter[i];
            }
        }
    }
}
