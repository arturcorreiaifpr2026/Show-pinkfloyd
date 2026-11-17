using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Sistemaparticulas : MonoBehaviour
{
    [Header("Tempo de execução")]
    [SerializeField] private float TempoParaDesativar = 5f;

    private ParticleSystem sistemaFumaca;
    private ParticleSystem.EmissionModule emissao;
    private bool contagemIniciada;

    private void Awake()
    {
        sistemaFumaca = GetComponent<ParticleSystem>();
        emissao = sistemaFumaca.emission;

        ParticleSystem.MainModule moduloPrincipal = sistemaFumaca.main;
        moduloPrincipal.playOnAwake = false;

        emissao.enabled = false;

        sistemaFumaca.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void IniciarContagem()
    {
        if (contagemIniciada)
        {
            return;
        }

        contagemIniciada = true;
        emissao.enabled = true;
        sistemaFumaca.Play();

        StartCoroutine(DesativarParticleSystem());
    }

    private IEnumerator DesativarParticleSystem()
    {
        yield return new WaitForSeconds(TempoParaDesativar);

        sistemaFumaca.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear);

        emissao.enabled = false;
    }
}
