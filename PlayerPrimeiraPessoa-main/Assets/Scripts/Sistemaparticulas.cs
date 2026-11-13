using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Sistemaparticulas : MonoBehaviour
{
    [Header("Tempo de execução")]
    [SerializeField] private float TempoParaDesativar = 5f;

    private ParticleSystem sistemaFumaca;

    private void Start()
    {
        sistemaFumaca = GetComponent<ParticleSystem>();

        sistemaFumaca.Play();

        StartCoroutine(DesativarParticleSystem());
    }

    private IEnumerator DesativarParticleSystem()
    {
        yield return new WaitForSeconds(TempoParaDesativar);

        sistemaFumaca.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear);

        sistemaFumaca.gameObject.SetActive(false);
    }
}
