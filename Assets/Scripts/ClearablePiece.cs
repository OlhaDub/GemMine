﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearablePiece : MonoBehaviour
    {
        public AnimationClip clearAnimation;

        public bool IsBeingCleared { get; private set; }

        protected GamePiece piece;


    private void Awake()
        {
        piece = GetComponent<GamePiece>();
        }

        public virtual void Clear()
        {
            piece.GameGridRef.level.OnPieceCleared(piece);
            IsBeingCleared = true;
            StartCoroutine(ClearCoroutine());
        }

        private IEnumerator ClearCoroutine()
        {
            var animator = GetComponent<Animator>();

            if (animator)
            {
                animator.Play(clearAnimation.name);

                yield return new WaitForSeconds(clearAnimation.length);

                Destroy(gameObject);
            }
        }
}

