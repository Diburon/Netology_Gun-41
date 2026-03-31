using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
	public class PositionSaver : MonoBehaviour
	{
        [System.Serializable]
        public struct Data
		{
			public Vector3 Position;
			public float Time;
		}

        [ReadOnly]
        [SerializeField]
        [Tooltip("Для заполнения этого поля нужно воспользоваться контекстным меню в инспекторе и командой “Create File”")]
		private TextAsset _json;

        [field: SerializeField]
        [HideInInspector]
        public List<Data> Records { get; private set; }

		private void Awake()
		{
            //todo comment: Что будет, если в теле этого условия не сделать выход из метода?
            //Ответ: код будет выполняться, пока не выбросит исключение.
            if (_json == null)
			{
				gameObject.SetActive(false);
				Debug.LogError("Please, create TextAsset and add in field _json");
				return;
			}
			
			JsonUtility.FromJsonOverwrite(_json.text, this);
            //todo comment: Для чего нужна эта проверка (что она позволяет избежать)?
            //Ответ: если поле = null, создаётся новый список с начальной ёмкостью 10 элементов.
            if (Records == null)
				Records = new List<Data>(10);
		}

		private void OnDrawGizmos()
		{
			//todo comment: Зачем нужны эти проверки (что они позволляют избежать)?
			//Прерывает выполнение, если коллекция пуста.
			if (Records == null || Records.Count == 0) return;
			var data = Records;
			var prev = data[0].Position;
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(prev, 0.3f);
            //todo comment: Почему итерация начинается не с нулевого элемента?
            //Ответ: если начать с 0, то будет попытка нарисовать линию от точки к самой себе.
            for (int i = 1; i < data.Count; i++)
			{
				var curr = data[i].Position;
				Gizmos.DrawWireSphere(curr, 0.3f);
				Gizmos.DrawLine(prev, curr);
				prev = curr;
			}
		}

#if UNITY_EDITOR
		[ContextMenu("Create File")]
		private void CreateFile()
		{
			//todo comment: Что происходит в этой строке?
			//Ответ: создание тектового файла.
			var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));
			//todo comment: Подумайте для чего нужна эта строка? (а потом проверьте догадку, закомментировав)
			//Ответ: Dispose снимает блокировку с файла.
			stream.Dispose();
			UnityEditor.AssetDatabase.Refresh();
			//В Unity можно искать объекты по их типу, для этого используется префикс "t:"
			//После нахождения, Юнити возвращает массив гуидов (которые в мета-файлах задаются, например)
			var guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset");
			foreach (var guid in guids)
			{
				//Этой командой можно получить путь к ассету через его гуид
				var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
				//Этой командой можно загрузить сам ассет
				var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
				//todo comment: Для чего нужны эти проверки?
				//Ответ: проверка на null и на соответвие имени.
				
				if(asset != null && asset.name == "Path")
				{
					_json = asset;
					UnityEditor.EditorUtility.SetDirty(this);
					UnityEditor.AssetDatabase.SaveAssets();
					UnityEditor.AssetDatabase.Refresh();
					//todo comment: Почему мы здесь выходим, а не продолжаем итерироваться?
					return;
					//Ответ: ассет найден, дальнейшая итерация не имеет смысла.
				}
			}
		}

		private void OnDestroy()
{

    if (_json != null)
    {
        var saveData = new { Records = this.Records };
        string jsonData = JsonUtility.ToJson(saveData, true);
        string assetPath = UnityEditor.AssetDatabase.GetAssetPath(_json);
        if (!string.IsNullOrEmpty(assetPath))
        {
            System.IO.File.WriteAllText(assetPath, jsonData);
            UnityEditor.AssetDatabase.Refresh();
            Debug.Log($"Данные сохранены в ассет: {assetPath}");
        }
        else
        {
            Debug.LogWarning("Не удалось получить путь к ассету _json");
        }
    }
    else
    {
        Debug.LogWarning("Поле _json не инициализировано — сохранение не выполнено");
    }
#endif
}

    }
}