﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralTileMapRenderer : MonoBehaviour
{
    public Tilemap tilemap;
    public int tileChunk = 16;
    public int tileSize = 16;
    public float groundScaleMinX = 6.2f;
    public float groundScaleMaxX = 6.6f;
    public float groundScaleMinY = 6.2f;
    public float groundScaleMaxY = 6.6f;
    public float groundScalingScaleX = 0.02f;
    public float groundScalingScaleY = 0.02f;
    public float groundBottomScale = 3.0f;
    public float groundBottomMin = 0.125f;
    public float groundBottomSide = 0.5f;
    public float backgroundScaleMinX = 1.5f;
    public float backgroundScaleMaxX = 1.7f;
    public float backgroundScaleMinY = 1.5f;
    public float backgroundScaleMaxY = 1.7f;
    public float backBrightScaleX = 0.1f;
    public float backBrightScaleY = 0.1f;
    public float backBrightness = 0.6f;
    public float backScalingScaleX = 0.015f;
    public float backScalingScaleY = 0.015f;
    public float crackScaleX = 0.12f;
    public float crackScaleY = 0.12f;
    public float cracksWidth = 0.2f;
    public float crackScatterX = 1.4f;
    public float crackScatterY = 1.4f;
    public float crackScatterWidth = 0.1f;
    public float grassRatio = 0.9f;
    public float grassScale = 0.064f;
    public float grassLength = 0.25f;
    public float grassLengthScale = 6.4f;
    public float bloodScaleX = 0.16f;
    public float bloodScaleY = 0.32f;
    public Vector2 bloodPosition = new Vector2(0, 0);
    public float minBloodDistance = 100.0f;
    public float minBloodRatio = 0.1f;
    public float maxBloodRatio = 0.7f;
    public float bloodSplatterScale = 2f;
    public float bloodSplatterRatio = 1f;
    public float vineRatio = 0.3f;
    public float vineScale = 0.1f;
    public float vineTextureRatio = 0.5f;
    public float vineTextureScaleX = 6.0f;
    public float vineTextureScaleY = 2.0f;
    public GameObject prefab;
    private GameObject[,] targets;

    void Start()
    {
        if (tilemap == null)
        {
            tilemap = GetComponent<Tilemap>();
        }
        TilemapRenderer tileRender = GetComponentInChildren<TilemapRenderer>();
        if (tileRender != null)
        {
            tileRender.enabled = false;
        }
        BoundsInt bounds = tilemap.cellBounds;
        int TextureSize = tileChunk * tileSize;
        int width = bounds.xMax - bounds.xMin;
        int height = bounds.yMax - bounds.yMin;
        int texturesWide = (int)Mathf.Ceil(width / (float)tileChunk);
        int texturesHigh = (int)Mathf.Ceil(height / (float)tileChunk);
        targets = new GameObject[texturesWide, texturesHigh];
        Vector3Int pos = new Vector3Int(0, 0, 0);
        for (int textureX = 0; textureX < texturesWide; textureX++)
        {
            for (int textureY = 0; textureY < texturesHigh; textureY++)
            {
                targets[textureX, textureY] = GameObject.Instantiate(prefab);
                targets[textureX, textureY].transform.localScale = new Vector3(tileChunk, tileChunk, 1);
                targets[textureX, textureY].transform.position = new Vector3(
                    bounds.xMin + (textureX * tileChunk) + (targets[textureX, textureY].transform.localScale.x / 2),
                    bounds.yMin + (textureY * tileChunk) + (targets[textureX, textureY].transform.localScale.y / 2),
                    0
                );

                Texture2D noiseTex = new Texture2D(TextureSize, TextureSize);
                Color[] pix = new Color[TextureSize * TextureSize];
                for (int x = 0; x < TextureSize; x++)
                {
                    for (int y = 0; y < TextureSize; y++)
                    {
                        pix[x + (y * TextureSize)] = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                    }
                }
                for (int tilePosX = 0; tilePosX < tileChunk; tilePosX++)
                {
                    for (int tilePosY = 0; tilePosY < tileChunk; tilePosY++)
                    {
                        pos.x = bounds.xMin + (textureX * tileChunk) + tilePosX;
                        pos.y = bounds.yMin + (textureY * tileChunk) + tilePosY;
                        bool hasTile = tilemap.HasTile(pos);
                        bool upTile = tilemap.HasTile(pos + Vector3Int.up);
                        bool downTile = tilemap.HasTile(pos + Vector3Int.down);
                        bool leftTile = tilemap.HasTile(pos + Vector3Int.left);
                        bool rightTile = tilemap.HasTile(pos + Vector3Int.right);

                        for (int x = 0; x < tileSize; x++)
                        {
                            for (int y = 0; y < tileSize; y++)
                            {
                                float totalX = (x + (tilePosX * tileSize) + (textureX * TextureSize)) / (float)tileSize;
                                float totalY = (y + (tilePosY * tileSize) + (textureY * TextureSize)) / (float)tileSize;

                                float bloodDist = ((new Vector2(totalX, totalY)) - bloodPosition).magnitude;
                                float bloodRatio = Mathf.Lerp(maxBloodRatio, minBloodRatio, Mathf.Clamp(bloodDist / minBloodDistance, 0, 1));
                                int pixelNumber = (x + (tilePosX * tileSize)) + ((y + (tilePosY * tileSize)) * TextureSize);
                                float bloodSample = (Mathf.PerlinNoise(bloodScaleX * totalX, bloodScaleY * totalY) - (1.0f - bloodRatio)) / bloodRatio;
                                float bloodSplatterSample = Mathf.PerlinNoise(bloodSplatterScale * totalX, bloodSplatterScale * totalY);
                                bool showingForeground = false;
                                if (hasTile)
                                {
                                    float groundScale = Mathf.PerlinNoise(groundScalingScaleX * totalX, groundScalingScaleY * totalY);
                                    float groundScaleX = Mathf.Lerp(groundScaleMinX, groundScaleMaxX, groundScale);
                                    float groundScaleY = Mathf.Lerp(groundScaleMinY, groundScaleMaxY, groundScale);
                                    float sample = Mathf.PerlinNoise(groundScaleX * totalX, groundScaleY * totalY);
                                    float bottomSample = Mathf.PerlinNoise(groundBottomScale * totalX, tilePosY);
                                    float minY = groundBottomMin;
                                    if (x < tileSize / 2)
                                    {
                                        if (!leftTile)
                                        {
                                            minY = Mathf.Lerp(groundBottomSide, groundBottomMin - (groundBottomSide - groundBottomMin), x / (float)tileSize);
                                        }
                                    } else
                                    {
                                        if (!rightTile)
                                        {
                                            minY = Mathf.Lerp(groundBottomMin - (groundBottomSide - groundBottomMin), groundBottomSide, x / (float)tileSize);
                                        }
                                    }
                                    if (downTile || y / (float)tileSize > bottomSample * minY)
                                    {
                                        showingForeground = true;
                                        if (!upTile)
                                        {
                                            if (bloodSample > (tileSize - y) / (float)tileSize || bloodSplatterSample * bloodSplatterRatio < bloodSample)
                                            {
                                                pix[pixelNumber] = new Color(0.3f + sample * 0.1f, 0f, 0f);
                                            }
                                            else
                                            {
                                                float grassSample = (Mathf.PerlinNoise(grassScale * totalX, grassScale * totalY) - (1.0f - grassRatio)) / grassRatio;
                                                if (grassSample > (tileSize - y) / (float)tileSize)
                                                {
                                                    pix[pixelNumber] = new Color(0f, 0.25f + sample * 0.25f, 0f);
                                                }
                                                else
                                                {
                                                    pix[pixelNumber] = new Color(0.4f + sample * 0.2f, 0.2f + sample * 0.25f, 0.1f + sample * 0.25f);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (bloodSplatterSample * bloodSplatterRatio < bloodSample)
                                            {
                                                pix[pixelNumber] = new Color(0.3f + sample * 0.1f, 0f, 0f);
                                            }
                                            else
                                            {
                                                pix[pixelNumber] = new Color(0.4f + sample * 0.2f, 0.2f + sample * 0.25f, 0.1f + sample * 0.25f);
                                            }
                                        }
                                    }
                                }

                                if (!showingForeground)
                                {
                                    bool showingGrass = false;
                                    if (downTile)
                                    {
                                        float grassSample = (Mathf.PerlinNoise(grassScale * totalX, grassScale * totalY) - (1.0f - grassRatio)) / grassRatio;
                                        if (grassSample > 0)
                                        {
                                            float grassLengthSample = Mathf.PerlinNoise(grassLengthScale * totalX, grassLengthScale * totalY) * grassLength;
                                            if (grassLengthSample > y / (float)tileSize)
                                            {
                                                showingGrass = true;
                                                if (bloodSample > 0 || bloodSplatterSample * bloodSplatterRatio < bloodSample)
                                                {
                                                    pix[pixelNumber] = new Color(0.25f + grassLengthSample * 0.1f, 0f, 0f);
                                                }
                                                else
                                                {
                                                    pix[pixelNumber] = new Color(0f, 0.25f + grassLengthSample * 0.25f, 0f);
                                                }
                                            }
                                        }
                                    }

                                    if (!showingGrass) {
                                        float vineSample = Mathf.PerlinNoise(vineScale * totalX, 0.0f);
                                        bool showingVines = false;
                                        if (vineSample > 1.0f - vineRatio)
                                        {
                                            float vineTextureSample = Mathf.PerlinNoise(vineTextureScaleX * totalX, vineTextureScaleY * totalY);
                                            if (vineTextureSample > 1.0f - vineTextureRatio)
                                            {
                                                showingVines = true;
                                                if (bloodSplatterSample * bloodSplatterRatio < bloodSample)
                                                {
                                                    pix[pixelNumber] = new Color(0.2f + vineTextureSample * 0.1f, 0f, 0f);
                                                }
                                                else
                                                {
                                                    pix[pixelNumber] = new Color(0.1f, vineTextureSample * 0.25f, 0f);
                                                }
                                            }
                                        }

                                        if (!showingVines)
                                        {
                                            float backgroundScale = Mathf.PerlinNoise(backScalingScaleX * totalX, backScalingScaleY * totalY);
                                            float backgroundScaleX = Mathf.Lerp(backgroundScaleMinX, backgroundScaleMaxX, backgroundScale);
                                            float backgroundScaleY = Mathf.Lerp(backgroundScaleMinY, backgroundScaleMaxY, backgroundScale);
                                            float sample = Mathf.PerlinNoise(backgroundScaleX * totalX, backgroundScaleY * totalY);

                                            float crackSample = Mathf.PerlinNoise(crackScaleX * totalX, crackScaleY * totalY);
                                            float crackScatter = Mathf.PerlinNoise(crackScatterX * totalX, crackScatterY * totalY);
                                            if (crackSample > 0.5f - cracksWidth * 0.5f && crackSample < 0.5f + cracksWidth * 0.5f && crackScatter > crackSample - crackScatterWidth * 0.5f && crackScatter < crackSample + crackScatterWidth * 0.5f)
                                            {
                                                if (bloodSplatterSample * bloodSplatterRatio < bloodSample)
                                                {
                                                    pix[pixelNumber] = new Color(0.2f, 0f, 0f);
                                                }
                                                else
                                                {
                                                    pix[pixelNumber] = new Color(0f, 0f, 0f);
                                                }
                                            }
                                            else
                                            {
                                                if (bloodSplatterSample * bloodSplatterRatio < bloodSample)
                                                {
                                                    pix[pixelNumber] = new Color(0.2f + sample * 0.1f, 0f, 0f);
                                                }
                                                else
                                                {
                                                    pix[pixelNumber] = new Color(0.3f + 0.2f * sample, 0.2f + 0.3f * sample, 0.2f);
                                                }
                                            }
                                        }

                                        pix[pixelNumber] *= Mathf.PerlinNoise(backBrightScaleX * totalX, backBrightScaleX * totalY) + backBrightness - 1.0f;
                                    }
                                }
                            }
                        }
                    }
                }

                // Copy the pixel data to the texture and load it into the GPU.
                noiseTex.SetPixels(pix);
                noiseTex.Apply(true, true);
                Renderer rend = targets[textureX, textureY].GetComponent<Renderer>();
                rend.material = new Material(rend.material);
                rend.material.mainTexture = noiseTex;
            }
        }
    }
}
