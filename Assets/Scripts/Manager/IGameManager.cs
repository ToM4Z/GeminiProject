
/*
 *  Class: IGameManager
 *  
 *  Description:
 *  Interface for all GameManagers
 *  
 *  Author: Carmelo Macrì (wrote by Thomas Voce)
*/

public interface IGameManager
{
    ManagerStatus status { get; }
    void Startup();
}
