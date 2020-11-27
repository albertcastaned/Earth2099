using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class ProjectileGun : MonoBehaviourPun
{
    public string gunName;
    public Text gunNameDisplay;

    public Sprite gunIcon;
    public Image gunImageDisplay;

    // Bullet
    public GameObject bullet;

    // Bullet force
    public float shootForce, upwardForce;

    // Gun stats
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap, numberOfMagazines;
    public bool allowButtonHold;

    int bulletsLeft, bulletsShot;

    // flags
    bool shooting, readyToShoot, reloading;

    // Reference
    public Transform attackPoint;
    
    // UI
    public GameObject muzzleFlash;
    public Text ammunitionDisplay;

    private LineRenderer lineRenderer;


    public bool allowInvoke = true;

    private void Awake()
    {
        // make sure magazine is full
        bulletsLeft = magazineSize;
        readyToShoot = true;
        lineRenderer = GetComponent<LineRenderer>();
        if(!photonView.IsMine)
        {
            lineRenderer.enabled = false;
        }
    }

    private void Update()
    {
        EventInput();

        UpdatePointer();
        
        // Set ammo display
        if (ammunitionDisplay != null)
        {
            ammunitionDisplay.text = 
                bulletsLeft / bulletsPerTap + " / " + (magazineSize / bulletsPerTap) * numberOfMagazines
            ;
        }

        // Set weapon name
        if(gunNameDisplay != null)
        {
            gunNameDisplay.text = gunName;
        }

        // Set weapon icon

        if (gunIcon != null && gunImageDisplay != null)
        {
            gunImageDisplay.sprite = gunIcon;
        }
    }


    void UpdatePointer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);

        lineRenderer.SetPosition(0, attackPoint.position);

        lineRenderer.SetPosition(1, targetPoint);
    }

    private void EventInput()
    {
        // Check if allowed to hold down button and take corresponding input
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);
        
        // Reloading
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
            Reload();
        if (readyToShoot && shooting & !reloading && bulletsLeft <= 0)
            Reload();

        // Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Check if ray hits something
            Vector3 targetPoint;
            if (Physics.Raycast(ray, out hit))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(75);

            var attackPosition = attackPoint.position;

            // Calculate direction from attackPoint to targetPoint
            Vector3 directionWithoutSpread = targetPoint - attackPosition;
            
            bulletsShot = 0;
            photonView.RPC(
                nameof(Shoot),
                RpcTarget.All,
                attackPosition,
                directionWithoutSpread
            );
        }
    }

    [PunRPC]
    public void Shoot(Vector3 from, Vector3 to)
    {

        readyToShoot = false;

        // Calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        // Calculate new direction with spread
        Vector3 directionSpread = to + new Vector3(x, y, 0);

        // Instantiate bullet/projectile
        GameObject currentBullet = Instantiate(bullet, from, Quaternion.identity);
        // Rotate bullet to shoot direction
        currentBullet.transform.forward = directionSpread.normalized;

        // Add forces to projectile
        currentBullet.GetComponent<Rigidbody>().AddForce(directionSpread.normalized * shootForce, ForceMode.Impulse);
        // TODO: Check how to add up force with MousePosition
        // currentBullet.GetComponent<Rigidbody>().AddForce(directionSpread.normalized * shootForce, ForceMode.Impulse);
        
        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, from, Quaternion.identity);
        }
        
        bulletsLeft--;
        bulletsShot++;

        if (allowInvoke)
        {
            Invoke(nameof(ResetShot), timeBetweenShooting);
            allowInvoke = false;
        }
        
        // if more than one bulletsPerShot make shure to repeat shoot function
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke(nameof(Shoot), timeBetweenShots);
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        if (numberOfMagazines == 0) return;
        reloading = true;
        Invoke(nameof(ReloadFinished), reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        numberOfMagazines -= 1;
        reloading = false;
    }

    public void moreBullets()
    {
        numberOfMagazines += 1;
        bulletsLeft = magazineSize;
    }
}
