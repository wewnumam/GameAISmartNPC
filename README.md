# GameAISmartNPC

Mata Kuliah Game Artificial Intelligence Semester 5 Prodi Teknologi Permainan STMM MMTC Yogyakarta 2023

## Latar Belakang  
Game ini merupakan game populer yang biasa dikenal sebagai "tactical shooter". "Tactical shooter adalah genre permainan video yang menekankan strategi, kerjasama tim, dan taktik dalam penggunaan senjata untuk mencapai tujuan permainan. Pengembangan game ini terinspirasi dari game Counter Strike dimana player menjadi counter terorist yang menyelamatkan sandera. Beberapa bertimbangan kami memilih game ini sebagai berikut:
- Gameplay lebih umum/mudah dipahami dibanding game lain
- Melibatkan jumlah NPC yang banyak dan aktif bergerak

## Konsep
Elemen | Deskripsi
--- | ---
Goals | Menyelamatkan sandra dan membawanya ke markas pemain.
Interaction | Third person control (WASD & Mouse).
Obstacle | Terorist akan berpatroli dan mengumpulkan sandera ke markasnya. <br> Terroist menyerang pemain jika pemain berada didekatnya.
Rules | Pemain harus menyelamatkan seluruh sandera.

## Teknologi yang Digunakan
  - [Unity](https://unity.com/), Merupakan lingkungan pengembangan perangkat lunak dan game engine yang memungkinkan pembuat game untuk membuat, mengelola, dan merancang aplikasi berbasis 2D maupun 3D.
  - [MonsterLove StateMachine](https://github.com/thefuntastic/Unity3d-Finite-State-Machine), Merupakan plugin untuk Unity yang menyediakan Finite State Machine (FSM) untuk membantu pengembang mengatur logika perpindahan keadaan dalam game dengan lebih terstruktur.
  - [Behavior Bricks](https://assetstore.unity.com/packages/tools/visual-scripting/behavior-bricks-74816), Merupakan alat visual scripting di Unity yang memfasilitasi pengembangan game dengan memungkinkan pengembang untuk merancang logika gameplay menggunakan blok-blok visual dan menghubungkannya secara intuitif.
  - [Starter Assets - Third Person Character Controller](https://assetstore.unity.com/packages/essentials/starter-assets-third-person-character-controller-urp-196526), Adalah sebuah paket asset di Unity yang menyediakan karakter pengendali pihak ketiga (third-person) sebagai dasar untuk pengembangan game, termasuk kontrol karakter, animasi, dan fitur-fitur dasar lainnya.

## Behavior hostage (FSM):

- Ketika berada di dekat markas pemain, mereka akan menuju markas pemain.
- Ketika berada di dekat markas teroris, mereka akan menuju markas teroris.
- Jika berada di dekat teroris, mereka akan mengikuti teroris.
- Jika berada di dekat pemain, mereka akan mengikuti pemain.
- Jika tidak ada pemain atau teroris di sekitar mereka, mereka akan menuju area perlindungan terdekat.

Perilaku sandera dikendalikan oleh sebuah finite state machine atau FSM yang diimplementasikan menggunakan library `MonsterLove.StateMachine` di Unity. FSM mengelola berbagai keadaan seperti inisialisasi, idle, dan mengikuti target.

Mari kita bahas komponen-komponen kunci dari kode tersebut:

1. **Serialized Fields:**
	```csharp
	[SerializeField] private NavMeshAgent agent;
	[SerializeField] private float maxFollowDistance = 10f;
	```
	Bidang ini diserialkan dan diekspos di Unity Inspector. `NavMeshAgent` adalah komponen untuk pencarian jalur dan navigasi pada permukaan NavMesh. `maxFollowDistance` adalah jarak maksimum di dalamnya sandera akan mengikuti target.
	
1. **Private Fields:**
	```csharp
	private Transform target;
	private bool isAtHQ;
	```
	`target` adalah referensi ke target saat ini yang sedang diikuti sandera, dan `isAtHQ` adalah bendera boolean yang menunjukkan apakah sandera berada di markas besar.

3. **Nested Class `Driver`:**
	```csharp
	public class Driver
	{
		public StateEvent Update;
		public StateEvent<Collider> OnTriggerEnter;
	}
	```
	Kelas bersarang ini mendefinisikan peristiwa (`Update` dan `OnTriggerEnter`) yang akan digunakan oleh state machine untuk berpindah antar keadaan.

4. **Enum `States`:**
	```csharp
	public enum States
	{
		Init,
		Idle,
		FollowTarget
	}
	```
	Enum ini mendefinisikan keadaan-keadaan yang mungkin dari state machine terbatas: `Init`, `Idle`, dan `FollowTarget`.

5. **Inisialisasi State Machine:**
	```csharp
	StateMachine<States, Driver> fsm;
	private void Awake()
	{
		fsm = new StateMachine<States, Driver>(this);
		fsm.ChangeState(States.Init);
	}
	```
	State machine diinisialisasi di dalam method `Awake` dengan keadaan awal diatur ke `Init`.

6. **Method Update:**
	```csharp
	private void Update()
	{
		fsm.Driver.Update.Invoke();
	}
	```
	 Method `Update` dari state machine dipanggil di dalam lingkaran `Update` Unity.

7. **Method OnTriggerEnter:**
	```csharp
	private void OnTriggerEnter(Collider collider)
	{
		// ... (Lihat kondisi if untuk peralihan keadaan)
		fsm.Driver.OnTriggerEnter.Invoke(collider);
	}
	```
	Method ini dipanggil ketika `Collider` sandera berinteraksi dengan collider lain. Bergantung pada tag collider, state machine berpindah ke keadaan yang berbeda.

8. **Method FollowTarget_Enter dan FollowTarget_Update:**
	```csharp
	private void FollowTarget_Enter()
	{
		// ... (Lihat kondisi untuk masuk ke keadaan FollowTarget)
	}
	
	private void FollowTarget_Update()
	{
		// ... (Lihat kondisi untuk memperbarui keadaan FollowTarget)
	}
	```
	Method ini khusus untuk keadaan `FollowTarget`. `FollowTarget_Enter` dipanggil saat memasuki keadaan, dan `FollowTarget_Update` dipanggil selama keadaan untuk menangani perilaku tertentu, seperti menetapkan tujuan untuk `NavMeshAgent` dan memeriksa apakah target berada dalam jarak tertentu.
  
9. **Method Idle_Enter:**
	```csharp
	private void Idle_Enter()
	{
		target = null;
		agent.ResetPath();
	}
	```
	Method ini dipanggil saat memasuki keadaan `Idle`. Ini mengatur kembali target menjadi null dan membersihkan jalur `NavMeshAgent`.

Secara keseluruhan, skrip ini menunjukkan penggunaan state machine terbatas untuk mengontrol perilaku objek permainan sandera di Unity. Sandera dapat berada dalam keadaan awal, keadaan idle, atau mengikuti target berdasarkan interaksi dengan objek permainan lain.

  
## Behavior terorist (Behavior Tree):
  
![](https://i.imgur.com/TGsuABs.png)

- Mereka akan menyerang pemain jika berada di dekat pemain.
- Jika berada di dekat sandera, mereka akan menculik sandera dan membawanya kembali ke markas teroris.
- Jika sudah mencapai markas teroris, mereka akan melakukan patroli.


## Gameplay

![](https://i.giphy.com/media/ByoGwL5kGQOyTD1a7E/source.gif)