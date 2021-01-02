using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

　　  //こっちは中身のマップ用のプレハブ
	public GameObject upWallPrefab;
	public GameObject rightWallPrefab;
	public GameObject downWallPrefab;
	public GameObject leftWallPrefab;
	public GameObject floorPrefab;

    public int mapX = 10;
    public int mapY = 10;

    /*
    【マップについて】
        マップは以下の情報を持つ、マス目情報の集合体とする
            ・4方に壁が存在するかどうか
        4方に壁が存在するかは、各bitが立っているかどうかで判別する
            ・1…上側に壁が存在する
            ・2…右側に壁が存在する
            ・4…下側に壁が存在する
            ・8…左側に壁が存在する
        現在居るマス目から、壁のある方向には進むことが出来ない
        [2,0]というマップが存在する場合、1マス目から2マス目へ移動することは出来ないが、2マス目から1マス目へ移動することはできる(一方通行の再現)
    */
    //これがマップの元になるデータ
    public int[][] map = new int[][] {
            new int[] {9, 5, 5, 5, 5, 1, 1, 1, 1, 3},
            new int[] {10, 9, 1, 1, 1, 0, 0, 0, 0, 2},
            new int[] {10, 8, 0, 0, 0, 0, 0, 0, 0, 2},
            new int[] {10, 8, 0, 0, 0, 0, 0, 0, 0, 2},
            new int[] {10, 8, 0, 0, 0, 0, 0, 0, 0, 2},
            new int[] {8, 0, 0, 0, 0, 0, 0, 0, 0, 2},
            new int[] {8, 0, 0, 0, 0, 0, 0, 0, 0, 2},
            new int[] {8, 0, 0, 0, 0, 0, 0, 0, 0, 2},
            new int[] {8, 0, 0, 0, 0, 0, 0, 0, 0, 2},
            new int[] {12, 4, 4, 4, 4, 4, 4, 4, 4, 6}
        };

    struct MapXY
    {
        int x;
        int y;
    }
    struct EventMap
    {
        MapXY map;
        int eventNo;
    }
    public MapXY[] = new int MapXY[]

    // Use this for initialization
    void Start () {
		// 引数にこれを入れてマップ生成する
		CreateMap(map);
	}
	
	//マップを作るメソッド
	void CreateMap(int[][] map)	{
        const float height = 0.5f;
        for (int y = 0; y < map.Length; y++) {
            for (int x = 0; x < map[y].Length; x++) {
                if ((map[y][x] & 1) == 1) {
                	Instantiate(upWallPrefab, new Vector3(x + 0.5f, height, mapY - y), Quaternion.identity);
                }
                if ((map[y][x] & 2) == 2) {
                	Instantiate(rightWallPrefab, new Vector3(x + 1, height, mapY - (y + 0.5f)), Quaternion.identity);
                }
                if ((map[y][x] & 4) == 4) {
                	Instantiate(downWallPrefab, new Vector3(x + 0.5f, height, mapY - (y + 1)), Quaternion.identity);
                }
                if ((map[y][x] & 8) == 8) {
                	Instantiate(leftWallPrefab, new Vector3(x + 0, height, mapY - (y + 0.5f)), Quaternion.identity);
                }
                //床
                Instantiate(floorPrefab, new Vector3(x + 0.5f, 0, mapY - (y + 0.5f)), Quaternion.identity);
                //天井
                Instantiate(floorPrefab, new Vector3(x + 0.5f, 1, mapY - (y + 0.5f)), Quaternion.identity);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
