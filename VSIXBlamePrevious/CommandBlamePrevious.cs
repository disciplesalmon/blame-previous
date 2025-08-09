using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace BlamePrevious
{
  /// <summary>
  /// Command handler
  /// </summary>
  internal sealed class CommandBlamePrevious
  {
    /// <summary>
    /// Command ID.
    /// </summary>
    public const int CommandId = 0x0100;

    /// <summary>
    /// Command menu group (command set GUID).
    /// </summary>
    public static readonly Guid CommandSet = new Guid("216449f6-ec2d-4fbd-846d-572347750232");

    /// <summary>
    /// VS Package that provides this command, not null.
    /// </summary>
    private readonly AsyncPackage package;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBlamePrevious"/> class.
    /// Adds our command handlers for menu (commands must exist in the command table file)
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    /// <param name="commandService">Command service to add command to, not null.</param>
    private CommandBlamePrevious(AsyncPackage package, OleMenuCommandService commandService)
    {
      this.package = package ?? throw new ArgumentNullException(nameof(package));
      commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

      var menuCommandID = new CommandID(CommandSet, CommandId);
      var menuItem = new MenuCommand(this.Execute, menuCommandID);
      commandService.AddCommand(menuItem);
    }

    /// <summary>
    /// Gets the instance of the command.
    /// </summary>
    public static CommandBlamePrevious Instance
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets the service provider from the owner package.
    /// </summary>
    private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
    {
      get
      {
        return this.package;
      }
    }

    /// <summary>
    /// Initializes the singleton instance of the command.
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    public static async Task InitializeAsync(AsyncPackage package)
    {
      // Switch to the main thread - the call to AddCommand in CommandBlamePrevious's constructor requires
      // the UI thread.
      await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

      OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
      Instance = new CommandBlamePrevious(package, commandService);
    }

    /// <summary>
    /// This function is the callback used to execute the command when the menu item is clicked.
    /// See the constructor to see how the menu item is associated with this function using
    /// OleMenuCommandService service and MenuCommand class.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event args.</param>
    private void Execute(object sender, EventArgs e)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      //This is a simple example of how to invoke an existing command behind a Visual Studio menu item.
      DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));
      dte?.ExecuteCommand("Team.Git.Annotate");

      //TODO: Reverse engineer where the current "Git > Blame (Annotate) > Annotate This Version" menu item lives.
      //TODO: Add a custom 'Blame Previous' menu item underneath the 'Annotate This Verison' menu item.
      //TODO: Reverse engineer what command 'Annotate This Version' actually does and do it better.

      //Here are other commands that may prove useful:
      /************************************************************************************************
       * Name: Team.Git.GoToGitActiveRepositories - ID: 4109 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.GoToGitSynchronization - ID: 4110 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.IgnoreItem - ID: 4131 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.OpenFromScc - ID: 4134 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Share - ID: 4135 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowAll - ID: 4138 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowSubset1ItemsOnly - ID: 4139 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowActivePullRequests - ID: 4164 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowCompletedPullRequests - ID: 4165 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowAbandonedPullRequests - ID: 4166 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowAllMyPullRequests - ID: 4167 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowTeamPullRequests - ID: 4168 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowOnlyMyPullRequests - ID: 4169 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.CommitOrStash - ID: 4208 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ToggleBranch - ID: 4237 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ClearToggledBranches - ID: 4238 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.DescribeChanges - ID: 4239 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Undo - ID: 4096 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.CompareWithUnmodified - ID: 4097 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Remove - ID: 4100 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.CompareWithPrevious - ID: 4103 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.CompareWithLatest - ID: 4104 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ViewCommitDetails - ID: 4105 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Checkout - ID: 4106 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.SwitchViewLayout - ID: 4111 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.SwitchDefaultAction - ID: 4112 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.SwitchBranchPublishState - ID: 4116 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Merge - ID: 4117 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Rename - ID: 4118 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.IgnoreExtension - ID: 4132 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ViewAndCreateBranch - ID: 4133 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.SwitchViewFilter - ID: 4136 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.OpenBrowser - ID: 4137 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.CommitAndPush - ID: 4140 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.CommitAndSync - ID: 4141 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Annotate - ID: 4142 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.AmendPreviousCommit - ID: 4143 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.CopyCommitID - ID: 4144 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Revert - ID: 4145 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.FetchSelection - ID: 4148 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.PullSelection - ID: 4149 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.PushSelection - ID: 4150 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.CreateTag - ID: 4152 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.AddToSourceControl - ID: 4153 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowBranches - ID: 4154 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowRemoteBranches - ID: 4155 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowTags - ID: 4156 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowGraph - ID: 4157 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowFullHistory - ID: 4158 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowFirstParentOnly - ID: 4159 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Rebase - ID: 4162 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.CreatePullRequest - ID: 4163 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.CherryPick - ID: 4174 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.GoToParent - ID: 4175 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.GoToChild - ID: 4176 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.SubmoduleUpdate - ID: 4177 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.SwitchSourceAndTarget - ID: 4178 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.CompareCommits - ID: 4179 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.UnsetUpstream - ID: 4180 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.CheckoutSourceBranch - ID: 4181 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.StashPush - ID: 4182 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.StashPop - ID: 4183 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.StashApply - ID: 4184 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.StashDrop - ID: 4185 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.StashPushKeepIndex - ID: 4187 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowStash - ID: 4188 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.RelateToChanges - ID: 4190 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.NewBranchFromWorkItem - ID: 4191 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.StashPopRestoreIndex - ID: 4192 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.StashApplyRestoreIndex - ID: 4193 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.KeepCurrent - ID: 4196 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.TakeIncoming - ID: 4197 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Stage - ID: 4205 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Unstage - ID: 4206 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.IgnoreAll - ID: 4210 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.CopyCommitDetails - ID: 4217 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.CheckoutDetachedHead - ID: 4231 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowIncomingOutgoingOnly - ID: 4250 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.NewRepository - ID: 4161 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Commit - ID: 4098 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Squash - ID: 4189 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.PushAllTags - ID: 4209 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.CreateDraftPullRequest - ID: 4241 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.AddSolutionToSourceControl - ID: 4099 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ViewHistory - ID: 4101 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Resolve - ID: 4119 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.UpgradeHostingProviderForCreatingDraft - ID: 4242 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ResetMixed - ID: 4172 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ResetHard - ID: 4173 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Clone - ID: 4120 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Publish - ID: 4130 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.GoToGitChanges - ID: 4108 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.WriteCommitGraphFile - ID: 4235 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Fetch - ID: 4146 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Pull - ID: 4147 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Push - ID: 4129 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Sync - ID: 4102 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.CreateBranch - ID: 4113 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ViewBranchHistory - ID: 4203 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ManageBranches - ID: 4107 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.OpenFileExplorer - ID: 4114 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.OpenCommandPrompt - ID: 4115 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ManageRemotes - ID: 4202 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.Settings - ID: 4204 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.StageSelectedRange - ID: 4232 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.UnstageSelectedRange - ID: 4233 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.RevertSelectedRange - ID: 4234 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.AddComment - ID: 4194 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.CollapseAllComments - ID: 4195 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.CopyLink - ID: 4244 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.CreateNewPullRequest - ID: 4212 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ViewPullRequests - ID: 4211 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ReviewPullRequest - ID: 4240 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowToolbarActions_Fetch - ID: 4213 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowToolbarActions_Pull - ID: 4214 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowToolbarActions_Push - ID: 4215 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       * Name: Team.Git.ShowToolbarActions_Sync - ID: 4216 - GUID: {57735D06-C920-4415-A2E0-7D6E6FBDFA99}
       ************************************************************************************************/
    }
  }
}
