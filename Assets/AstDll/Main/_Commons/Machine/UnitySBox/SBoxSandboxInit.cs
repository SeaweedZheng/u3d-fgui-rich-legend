using SBoxApi;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SBoxSandboxInit : MonoSingleton<SBoxSandboxInit>
{
    private UnityAction onComplete;

    public void Init(string matchIp, UnityAction onComplete = null)
    {
        MatchDebugManager.Instance.InitUdpNet(matchIp);

        this.onComplete = onComplete;
        if (!transform.TryGetComponent(out SBox Sbox))
            gameObject.AddComponent<SBox>();

        if(!Application.isEditor)
            SBox.Init();

        AddEventListener();
        StartCoroutine(CheckSBoxSandboxReady());
    }


    private void AddEventListener()
    {
        EventCenter.Instance.AddEventListener<int>(EventHandle.CHECK_SBOX_SANBOX_READY, OnSBoxSandboxReady);
        EventCenter.Instance.AddEventListener<int>(SBoxEventHandle.SBOX_SADNBOX_RESET, OnSBoxSandboxReset);
    }

    private void OnSBoxSandboxReady(int machineId)
    {
        //SBoxModel.Instance.macId = machineId;
        SBoxModel.Instance.isSboxSandboxReady = true;
    }

    private void OnSBoxSandboxReset(int result)
    {

        //LoadingPage.Instance.Next(LoadingProgress.CONNECT_MACHINE, "SBoxSandbox Reset...");

        if (result != 0)
        {
            DebugUtils.LogError("SBoxError When Call SBoxReset:" + result);
            return;
        }
        onComplete?.Invoke();
    }

    private IEnumerator CheckSBoxSandboxReady()
    {
        //LoadingPage.Instance.Next(LoadingProgress.CONNECT_MACHINE, "Check SBoxSandbox ready...");
        while (!SBoxModel.Instance.isSboxSandboxReady)
        {
            SBoxModel.Instance.isSboxSandboxReady = SBoxSandbox.Ready();
            yield return new WaitForSeconds(0.5f);
        }

        if (Application.isEditor)
            MatchDebugManager.Instance.SendUdpMessage(SBoxEventHandle.SBOX_SADNBOX_RESET);
        else
        {
            SBoxSandbox.Reset();
            //SandboxController.Instance.Init();
            SBoxSandboxListener.Instance.Init();
        }
    }
}
