using Audio;
using System;
using System.Collections;
using UnityEngine;

namespace Santa
{
    [RequireComponent(typeof(AudioSource))]
    //[ExecuteInEditMode]
    public class Door : MonoBehaviour, IInteractable, ITrigger, IMemento
    {
        public bool open = false;
        public bool locked = true;
        public bool canUsedAsGhost = true;
        public GameObject doorPrefab;
        public AudioPicker audioPicker;

        [Header("movement")]
        public float duration = 3f;
        public Vector3 translation = Vector3.zero;
        public Vector3 rotation = new Vector3(0, 90, 0);

        public Vector3 startPosition;
        public Quaternion startRotation;
        private new AudioSource audio;
        private float timer;

        private bool isOpen;

        void Start()
        {
            isOpen = open;
            if (!doorPrefab) return;
            if (!open)
            {
                startPosition = doorPrefab.transform.position;
                startRotation = doorPrefab.transform.rotation;
            }
            audio = GetComponent<AudioSource>();
            GameManager.Instance.AddMementoObject(this);
        }

        /*#if UNITY_EDITOR
                private void Start()
                {
                    if (Application.isPlaying)
                        GameManager.Instance.AddMementoObject(this);
                }
        #endif

        #if UNITY_EDITOR
                public void Update()
                {
                    if (!Application.isPlaying)
                    {
                        if (isOpen != open)
                        {
                            EditorTrigger();
                        }
                    }
                }
        #endif*/

        public bool CanBeTriggered()
        {
            return true;
        }

        public void Interact(MonoBehaviour user)
        {
            if (CanInteractWith(user))
            {
                Trigger(user, TriggerCommand.Toggle);
            }
        }

        public bool CanInteractWith(MonoBehaviour user)
        {
            if (GameManager.Instance.unlockedDoors) return true;
            return !locked;
        }

        public string GetInteractionHint()
        {
            return locked ? "Lässt sich nicht bewegen" : isOpen ? "Schließen" : "Öffnen";
        }

        [ContextMenu("Trigger")]
        public void EditorTrigger()
        {
            isOpen = !isOpen;
            if (isOpen)
            {
                doorPrefab.transform.position = startPosition + translation;
                doorPrefab.transform.rotation = Quaternion.Euler(startRotation.eulerAngles + rotation);
            }
            else
            {
                doorPrefab.transform.position = startPosition;
                doorPrefab.transform.rotation = startRotation;
            }
        }

        [ContextMenu("Reset Transforms")]
        public void ResetTransforms()
        {
            isOpen = open;
            if (!doorPrefab) return;
            if (open)
            {
                startPosition = (doorPrefab.transform.position - translation).Round();
                startRotation = Quaternion.Euler((doorPrefab.transform.rotation.eulerAngles - rotation).Round());
            }
            else
            {
                startPosition = doorPrefab.transform.position;
                startRotation = doorPrefab.transform.rotation;
            }
        }

        public void SetOpenState(bool open)
        {
            StopAllCoroutines();
            isOpen = open;
            if (open)
            {
                doorPrefab.transform.position = startPosition + translation;
                doorPrefab.transform.rotation = Quaternion.Euler(startRotation.eulerAngles + rotation);
            }
            else
            {
                doorPrefab.transform.position = startPosition;
                doorPrefab.transform.rotation = startRotation;
            }
        }

        public void ForceOpen(bool playSound)
        {
            if (isOpen) return;
            isOpen = true;
            if (playSound && audio)
            {
                audioPicker.Play(audio);
            }
            StartCoroutine(Move());
        }

        public void ForceClose(bool playSound)
        {
            if (!isOpen) return;
            isOpen = false;
            if (playSound && audio)
            {
                audioPicker.Play(audio);
            }
            StartCoroutine(Move());
        }

        public void Trigger(MonoBehaviour user, TriggerCommand cmd)
        {
            switch (cmd)
            {
                case TriggerCommand.Toggle: isOpen = !isOpen; break;
                case TriggerCommand.ForceOn: isOpen = true; break;
                case TriggerCommand.ForceOff: isOpen = false; break;
            }
            if (audio)
            {
                audioPicker.Play(audio);
            }
            StartCoroutine(Move());
        }

        private IEnumerator Move()
        {
            timer = 0;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                if (isOpen)
                {
                    doorPrefab.transform.position = startPosition + Vector3.Lerp(Vector3.zero, translation, timer / duration);
                    doorPrefab.transform.rotation = Quaternion.Euler(startRotation.eulerAngles + Vector3.Lerp(Vector3.zero, rotation, timer / duration));
                }
                else
                {
                    doorPrefab.transform.position = startPosition + Vector3.Lerp(translation, Vector3.zero, timer / duration);
                    doorPrefab.transform.rotation = Quaternion.Euler(startRotation.eulerAngles + Vector3.Lerp(rotation, Vector3.zero, timer / duration));
                }
                yield return null;
            }
        }

        public void SetData(MementoData data)
        {
            isOpen = data.a != 0;
            locked = data.b != 0;
            SetOpenState(isOpen);
        }

        public MementoData GetData()
        {
            return new MementoData(isOpen, locked);
        }
    }
}
